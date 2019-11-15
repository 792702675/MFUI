import React from 'react';
import { Form, Input, InputNumber, DatePicker, TreeSelect, Tag, Descriptions } from 'antd';
const create = Form.create;
const FormItem = Form.Item;
import * as api from '@/api/api';
import { call } from '@/api/apiUtil';
import PageList from '@/components/PageList/PageList';
import moment from 'moment';

class Client extends React.Component {
	constructor() {
		super();
		this.state = { policies: [] };
	}

	componentDidMount() {
		call({
			method: new api.ClientApi().getPolicies
		}).then(({ result, success }) => {
			if (success) {
				this.setState({
					policies: this.mapPolicies(result)
				});
			}
		});
	}

	mapPolicies = (policies) =>
		policies.map((x) => ({
			title: x.displayName,
			value: x.name,
			key: x.name,
			children: this.mapPolicies(x.children)
		}));

	getDisplayNameByNane = (name) => {
		let displayName;

		const fun = (arr) => {
			arr.forEach((x) => {
				if (x.value == name) {
					displayName = x.title;
					return;
				}
				fun(x.children);
			});
		};
		fun(this.state.policies);
		return displayName;
	};

	render() {
		const formCol = {
			labelCol: { span: 8 },
			wrapperCol: { span: 12 }
		};

		return (
			<PageList
				ref={(pg) => (this.pageList = pg)}
				form={this.props.form}
				mapGetAll={(x) => ({ ...x, id: x.clientId })}
				onModalOpen={(record) => {
					if (record === null) {
						call({
							method: new api.ClientApi().getSecret
						}).then(({ result, success }) => {
							if (success) {
								this.props.form.setFieldsValue({
									secrets: [ result ]
								});
							}
						});
					}
				}}
				onSaveing={(values) => {
					values.clientId = values.id;
					return values;
				}}
				modalProps={{ width: 800 }}
				api={new api.ClientApi().getAllAsync}
				deleteApi={new api.ClientApi().deleteAsync}
				updateApi={new api.ClientApi().updateAsync}
				createApi={new api.ClientApi().createAsync}
				renderFormNode={(record, getFieldDecorator) => (
					<Form>
						<FormItem label="名称" {...formCol}>
							{getFieldDecorator('clientName', {
								initialValue: record.clientName,
								rules: [ { required: true, message: '请填写名称' } ]
							})(<Input />)}
						</FormItem>
						{this.pageList.state.isAdd && (
							<FormItem label="secret" help="请记好secret在保存，一旦保存，就不能看到他了" {...formCol}>
								{getFieldDecorator('secrets')(<Input disabled />)}
							</FormItem>
						)}
						<FormItem label="Scope" {...formCol}>
							{getFieldDecorator('allowedScopes', {
								initialValue: record.allowedScopes
							})(
								<TreeSelect
									treeData={this.state.policies}
									treeCheckable={true}
									treeDefaultExpandAll={true}
								/>
							)}
						</FormItem>
					</Form>
				)}
				// filterProps={{
				// 	filters: [
				// 		{
				// 			name: 'name',
				// 			displayName: '名称',
				// 			option: 'like'
				// 		}
				// 	],
				// 	searchProvide: 'sql'
				// }}
				listRenderItemType="card"
				renderItem={(item) => (
					<Descriptions title={item.clientName} column={1}>
						<Descriptions.Item label="clientId">{item.id}</Descriptions.Item>
						<Descriptions.Item label="scope">
							{item.allowedScopes.map((x) => <Tag>{this.getDisplayNameByNane(x)}</Tag>)}
						</Descriptions.Item>
					</Descriptions>
				)}
			/>
		);
	}
}
export default create()(Client);
