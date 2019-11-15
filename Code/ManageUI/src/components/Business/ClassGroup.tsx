import React from 'react';
import {
	Select,
	Card,
	Row,
	Col,
	Button,
	Menu,
	Table,
	Input,
	Icon,
	message,
	Modal,
	Form,
	Dropdown,
	InputNumber
} from 'antd';
const create = Form.create;
const FormItem = Form.Item;
import { call } from '@/api/apiUtil';
import * as api from '@/api/api';
import GenerateCode from '../System/GenerateCode';
import EnumSelect from '../System/EnumSelect';
import { remoteUrl } from '@/utils/url';

class Index extends React.Component {
	static defaultProps = {
		isTeacher: false
	};

	state = {
		classGroups: [],
		createVisible: false,
		updateVisible: false,
		batchCreateStudentVisible: false,
		createStudentVisible: false
	};

	componentDidMount() {
		this.getClassGroups();
	}

	getClassGroups = () => {
		call({
			method: new api.ClassGroupApi().getAll,
			payload: { maxResultCount: 1000, useTeacher: this.props.isTeacher }
		}).then(({ result }) => {
			this.setState({ classGroups: result.items });
			if (
				this.state.selectClassGroup &&
				this.state.classGroups.find((x) => x.id == this.state.selectClassGroup.id)
			) {
				this.setState({
					selectClassGroup: this.state.classGroups.find((x) => x.id == this.state.selectClassGroup.id)
				});
			} else {
				this.setState({
					selectClassGroup: null
				});
			}
		});
	};

	handleClassGroupChange = (item) => {
		this.setState({ selectClassGroup: this.state.classGroups.find((x) => x.id == item.key), students: null });
		call({
			method: new api.ClassGroupApi().getStudents,
			payload: { id: item.key }
		}).then(({ result }) => {
			this.setState({ students: result });
		});
	};

	updateStudentName = (classGroupId, studentId, e) => {
		call({
			method: new api.ClassGroupApi().updateStudentName,
			payload: { classGroupId, studentId, displayName: e.target.value }
		}).then(({ result }) => {
			message.info('修改成功');
			// this.handleClassGroupChange({ key: classGroupId });
		});
	};

	rowSelection = {
		onChange: (selectedRowKeys, selectedRows) => {
			this.setState({ selectedStudentIds: selectedRowKeys });
		}
	};

	render() {
		const { form, isTeacher } = this.props;
		const {
			classGroups,
			selectClassGroup,
			students,
			createVisible,
			updateVisible,
			batchCreateStudentVisible,
			createStudentVisible,
			selectedStudentIds
		} = this.state;
		const { getFieldDecorator } = form;
		const formCol = {
			labelCol: { span: 8 },
			wrapperCol: { span: 12 }
		};
		return (
			<div>
				<Row gutter={16}>
					<Col span={6}>
						<Card
							title="全部班组"
							extra={
								<Button
									type="primary"
									size="small"
									onClick={() => {
										form.resetFields();
										this.setState({ createVisible: true });
									}}
								>
									创建班组
								</Button>
							}
						>
							<Menu
								mode="inline"
								onClick={this.handleClassGroupChange}
								selectedKeys={selectClassGroup && [ selectClassGroup.id + '' ]}
							>
								{classGroups.map((x) => <Menu.Item key={x.id}>{x.displayName}</Menu.Item>)}
							</Menu>
						</Card>
					</Col>
					<Col span={18}>
						{selectClassGroup ? (
							<Card
								title={selectClassGroup.displayName}
								extra={
									<span style={{ fontSize: '16px' }}>
										<Dropdown
											trigger={[ 'click' ]}
											overlay={
												<Menu>
													<Menu.Item
														onClick={() => {
															form.resetFields();
															this.setState({ createStudentVisible: true });
														}}
													>
														导入已有账号
													</Menu.Item>
													<Menu.Item
														onClick={() => {
															form.resetFields();
															this.setState({ batchCreateStudentVisible: true });
														}}
													>
														批量添加
													</Menu.Item>
												</Menu>
											}
										>
											<Icon type="user-add" />
										</Dropdown>
										<Dropdown
											trigger={[ 'click' ]}
											overlay={
												<Menu>
													<Menu.Item
														onClick={() => {
															form.setFieldsValue({
																name: selectClassGroup.name,
																teacherId: selectClassGroup.teacherId
															});
															this.setState({ updateVisible: true });
														}}
													>
														修改班组
													</Menu.Item>
													<Menu.Item
														onClick={() => {
															call({
																method: new api.ClassGroupApi().getStudentToExcel,
																payload: { id: selectClassGroup.id + '' }
															}).then(({ result }) => {
																window.open(
																	`${remoteUrl}/api/File/DownloadTempFile?filename=${result.fileName}&fileType=${result.fileType}&fileToken=${result.fileToken}`
																);
															});
														}}
													>
														生成名单
													</Menu.Item>
													<Menu.Item
														onClick={() => {
															Modal.confirm({
																okText: '确定',
																cancelText: '取消',
																content: '确定解散吗？',
																onOk: () => {
																	call({
																		method: new api.ClassGroupApi()._delete,
																		payload: { id: selectClassGroup.id }
																	}).then(({ result }) => {
																		message.info('解散成功');
																		this.getClassGroups();
																	});
																}
															});
														}}
													>
														解散班组
													</Menu.Item>
												</Menu>
											}
										>
											<Icon type="menu" style={{ marginLeft: '8px' }} />
										</Dropdown>
									</span>
								}
							>
								<div>
									{selectClassGroup.teacherDisplayName}
									<span style={{ marginLeft: '16px' }}>学生数{students ? students.length : ' '}名</span>
								</div>
								<Button
									style={{ margin: '8px 0' }}
									onClick={() => {
										if (!selectedStudentIds || selectedStudentIds.length == 0) {
											message.info('还没有选择学生');
											return;
										}
										Modal.confirm({
											okText: '确定',
											cancelText: '取消',
											content: '确定移除吗？',
											onOk: () => {
												call({
													method: new api.ClassGroupApi().removeStudent,
													payload: {
														classGroupId: selectClassGroup.id,
														students: selectedStudentIds
													}
												}).then(({ result }) => {
													message.info('移除成功');
													this.handleClassGroupChange({
														key: selectClassGroup.id
													});
													this.setState({ selectedStudentIds: null });
												});
											}
										});
									}}
								>
									批量移除
								</Button>
								<Table
									loading={!students}
									rowSelection={this.rowSelection}
									rowKey={(x) => x.id}
									pagination={false}
									columns={[
										{
											title: '用户名',
											dataIndex: 'userName'
										},
										{
											title: '姓名',
											dataIndex: 'name'
										},
										{
											title: '展示姓名',
											render: (t, r) => (
												<Input
													onPressEnter={(e) =>
														this.updateStudentName(selectClassGroup.id, r.id, e)}
													defaultValue={r.displayName || r.name}
													onBlur={(e) => this.updateStudentName(selectClassGroup.id, r.id, e)}
												/>
											)
										},
										{
											title: '操作',
											render: (t, r) => (
												<a
													onClick={() => {
														Modal.confirm({
															okText: '确定',
															cancelText: '取消',
															content: '确定移除吗？',
															onOk: () => {
																call({
																	method: new api.ClassGroupApi().removeStudent,
																	payload: {
																		classGroupId: selectClassGroup.id,
																		students: [ r.id ]
																	}
																}).then(({ result }) => {
																	message.info('移除成功');
																	this.handleClassGroupChange({
																		key: selectClassGroup.id
																	});
																});
															}
														});
													}}
												>
													<Icon type="delete" />
												</a>
											)
										}
									]}
									dataSource={students}
								/>
							</Card>
						) : (
							<Card title="请选择左侧班组" />
						)}
					</Col>
				</Row>

				<Modal
					visible={createVisible}
					title="创建班组"
					maskClosable={false}
					onCancel={() => {
						this.setState({ createVisible: false });
					}}
					onOk={() => {
						form.validateFields((err, values) => {
							if (!err) {
								call({
									method: new api.ClassGroupApi().create,
									payload: values
								}).then(({ result }) => {
									message.info('添加成功');
									this.setState({ createVisible: false, selectClassGroup: result, students: [] });
									this.getClassGroups();
								});
							}
						});
					}}
				>
					<Form>
						<FormItem label="班组名称" {...formCol}>
							{getFieldDecorator('name', {
								rules: [ { required: true, message: '请填写班组名称' } ]
							})(<Input />)}
						</FormItem>
						<FormItem label="班组编号" {...formCol}>
							{getFieldDecorator('code', {
								rules: [ { required: true, message: '请生成班组编号' } ]
							})(<GenerateCode />)}
						</FormItem>
						{!isTeacher && (
							<FormItem label="导师" {...formCol}>
								{getFieldDecorator('teacherId', {
									rules: [ { required: true, message: '请选择导师' } ]
								})(
									<EnumSelect
										api={new api.UserApi().getTeacherList}
										apiResultMap={(x) => ({ name: x.userName + '(' + x.name + ')', value: x.id })}
									/>
								)}
							</FormItem>
						)}
					</Form>
				</Modal>
				<Modal
					visible={updateVisible}
					title="修改班组"
					maskClosable={false}
					onCancel={() => {
						this.setState({ updateVisible: false });
					}}
					onOk={() => {
						form.validateFields((err, values) => {
							if (values.name) {
								call({
									method: new api.ClassGroupApi().update,
									payload: {
										name: values.name,
										id: selectClassGroup.id,
										teacherId: isTeacher ? null : values.teacherId
									}
								}).then(({ result }) => {
									message.info('修改成功');
									this.setState({ updateVisible: false });
									this.getClassGroups();
								});
							}
						});
					}}
				>
					<Form>
						<FormItem label="班组名称" {...formCol}>
							{getFieldDecorator('name', {
								rules: [ { required: true, message: '请填写班组名称' } ]
							})(<Input />)}
						</FormItem>
						{!isTeacher && (
							<FormItem label="选择导师" {...formCol}>
								{getFieldDecorator('teacherId', {
									rules: [ { required: true, message: '请选择导师' } ]
								})(
									<EnumSelect
										api={new api.UserApi().getTeacherList}
										optionRender={(x, i) => (
											<Select.Option {...x} key={i} value={x.id}>
												{x.name}
											</Select.Option>
										)}
									/>
								)}
							</FormItem>
						)}
					</Form>
				</Modal>
				{batchCreateStudentVisible && (
					<Modal
						visible={batchCreateStudentVisible}
						title="批量添加学生"
						maskClosable={false}
						onCancel={() => {
							this.setState({ batchCreateStudentVisible: false });
						}}
						onOk={() => {
							form.validateFields((err, values) => {
								if (values.studentCount) {
									call({
										method: new api.ClassGroupApi().addStudentOfRule,
										payload: {
											studentCount: values.studentCount,
											classGroupId: selectClassGroup.id
										}
									}).then(({ result }) => {
										message.info('添加成功');
										this.setState({ batchCreateStudentVisible: false });
										this.handleClassGroupChange({ key: selectClassGroup.id });
									});
								}
							});
						}}
					>
						<Form>
							<FormItem label="学生个数" {...formCol}>
								{getFieldDecorator('studentCount', {
									rules: [ { required: true, message: '请填写学生个数' } ]
								})(<InputNumber min={1} />)}
							</FormItem>
						</Form>
					</Modal>
				)}
				{createStudentVisible && (
					<Modal
						visible={createStudentVisible}
						title="导入已有学生账号"
						maskClosable={false}
						onCancel={() => {
							this.setState({ createStudentVisible: false });
						}}
						onOk={() => {
							form.validateFields((err, values) => {
								if (values.students) {
									call({
										method: new api.ClassGroupApi().addStudent,
										payload: {
											students: values.students,
											classGroupId: selectClassGroup.id
										}
									}).then(({ result }) => {
										message.info('添加成功');
										this.setState({ createStudentVisible: false });
										this.handleClassGroupChange({ key: selectClassGroup.id });
									});
								}
							});
						}}
					>
						<Form>
							<FormItem label="学生" {...formCol}>
								{getFieldDecorator('students', {
									rules: [ { required: true, message: '请选择学生' } ]
								})(
									<EnumSelect
										api={new api.UserApi().getStudentList}
										apiResultMap={(x) => ({ name: x.userName + '(' + x.name + ')', value: x.id })}
										mode="multiple"
									/>
								)}
							</FormItem>
						</Form>
					</Modal>
				)}
			</div>
		);
	}
}
export default create()(Index);
