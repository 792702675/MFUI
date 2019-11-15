import React from 'react';
import { List, Spin, Button, Card, Icon, Modal, Form } from 'antd';
const create = Form.create;
import * as api from '../../api/api';
import { call } from '../../api/apiUtil';
import Filter from '../Filter/Filter';
import PermissionWrapper from '../PermissionWrapper/PermissionWrapper';
import { isArgumentPlaceholder } from '@babel/types';
import { isArray } from 'util';

export default class PageList extends React.Component {
	static defaultProps = {
		getData: () => {},
		mapGetAll: (x) => x
	};
	constructor() {
		super();
		this.state = {
			items: [],
			pagination: {
				total: 0,
				current: 1,
				pageSize: 10,
				showSizeChanger: true,
				showQuickJumper: true,
				showTotal: (total) => `共 ${total} 条`,
				size: 'large'
			},
			filter: null,
			sort: null,
			loading: false,

			modalVisible: false,
			record: {},
			modalText: '',
			isAdd: true,
			saveLoading: false
		};
	}

	componentDidMount() {
		this.getData(
			this.state.pagination,
			{ ...this.state.filter || {}, ...this.props.cuntomFilter || {} },
			this.state.sort
		);
		this.props.getData(() => {
			this.getData(this.state.pagination, this.state.filter, this.state.sort);
		});
	}

	getData = (pagination, filter, sort) => {
		this.setState({
			loading: true
		});
		call({
			method: this.props.api,
			payload: {
				...this.props.payload,
				...filter,
				sort,
				skipCount: (pagination.current - 1) * pagination.pageSize,
				maxResultCount: pagination.pageSize
			}
		}).then(({ result, success }) => {
			if (success) {
				if (result.items) {
					this.setState({
						filter,
						sort,
						pagination: {
							...pagination,
							total: result.totalCount
						},
						items: result.items.map(this.props.mapGetAll),
						loading: false
					});
				} else {
					this.setState({
						filter,
						sort,
						pagination: null,
						items: result.map(this.props.mapGetAll),
						loading: false
					});
				}
			}
		});
	};

	onPageChange = (page, pageSize) => {
		this.getData({ ...this.state.pagination, current: page, pageSize }, this.state.filter, this.state.sort);
	};

	onFilter = (filter) => {
		this.getData({ ...this.state.pagination, current: 1 }, filter, this.state.sort);
	};

	onSort = (sort) => {
		this.getData({ ...this.state.pagination, current: 1 }, this.state.filter, sort);
	};

	refreshData = () => {
		this.getData({ ...this.state.pagination, current: 1 }, this.state.filter, this.state.sort);
	};

	showAddModal = () => {
		if (this.props.onModalOpen) {
			this.props.onModalOpen(null);
		}
		this.setState({
			modalVisible: true,
			modalText: '添加',
			isAdd: true,
			record: this.props.defaultRecord || {}
		});
	};

	showEditModal = (record) => {
		if (this.props.defaultRecord) {
			record = { ...this.props.defaultRecord, ...record };
		}
		if (this.props.onModalOpen) {
			this.props.onModalOpen(record);
		}
		this.setState({
			modalVisible: true,
			record: record,
			modalText: '编辑',
			isAdd: false
		});
		this.props.form.resetFields();
		if (this.props.useServiceUpdate) {
			call({
				method: this.props.getApi,
				payload: {
					id: record.id
				}
			}).then(({ result, success }) => {
				if (success) {
					for (var key in result) {
						this.props.form.getFieldDecorator(key, { initialValue: result[key] });
					}
				}
			});
		} else {
			for (var key in record) {
				this.props.form.getFieldDecorator(key, { initialValue: record[key] });
			}
		}
	};
	save = () => {
		this.props.form.validateFields((err, values) => {
			if (!err) {
				if (!this.state.isAdd) {
					values.id = this.state.record.id;
				}
				if (this.props.onSaveing) {
					values = this.props.onSaveing(values);
				}
				if (this.props.rules) {
					this.props.rules(values);
				} else {
					call({
						method: this.state.isAdd ? this.props.createApi : this.props.updateApi,
						payload: values
					}).then(({ result, success }) => {
						if (success) {
							if (this.props.onSaved) {
								this.props.onSaved(result, this.state.isAdd);
							}
							this.refreshData();
							this.setState({
								modalVisible: false
							});
						}
					});
				}
			}
		});
	};

	delete = (id) => {
		Modal.confirm({
			okText: '确定',
			cancelText: '取消',
			title: '警告',
			content: (
				<div>
					<div>你确定要删除吗?</div>
				</div>
			),
			width: 350,
			onOk: () =>
				call({
					method: this.props.deleteApi,
					payload: { id }
				}).then(({ result, success }) => {
					if (success) {
						this.refreshData();
					}
				})
		});
	};

	deleteBatch = () => {
		alert('未实现');
	};

	toExcel = () => {
		call({
			method: this.props.toExcelApi,
			payload: {
				...this.props.payload,
				...this.state.filter,
				sort: this.state.sort
			}
		}).then(({ result, success }) => {
			if (success) {
				window.open(
					`${remoteUrl}/api/File/DownloadTempFile?filename=${result.fileName}&fileType=${result.fileType}&fileToken=${result.fileToken}`
				);
			}
		});
	};

	buttonStyle = {
		marginBottom: '12px',
		marginRight: '12px'
	};

	render() {
		const {
			form,
			listProps = {},
			paginationProps = {},
			filterProps = {},
			modalProps = {},

			renderItem,
			renderFormNode,

			createApi,
			updateApi,
			deleteApi,
			toExcelApi,
			deleteBatchApi,
			createPermission,
			updatePermission,
			deletePermission,
			toExcelPermission,

			perToolButtons,
			postToolButtons,

			listRenderItemType,
			listItemProps = {},
			cardProps = {},
			actionLocation = 'left',
			actions = []
		} = this.props;
		const { items, pagination, loading, modalVisible, modalText, saveLoading, record } = this.state;

		let listRenderItem = renderItem;
		switch (listRenderItemType) {
			case 'card':
				const getCardActions = (item) => {
					let _actions = [];
					if (updateApi && PermissionWrapper.isGrant) {
						_actions.push(<Icon type="edit" onClick={() => this.showEditModal(item)} />);
					}
					if (updateApi && PermissionWrapper.isGrant) {
						_actions.push(<Icon type="delete" onClick={() => this.delete(item.id)} />);
					}
					_actions =
						actionLocation === 'right'
							? [ ..._actions, ...actions.map((x) => x(item)) ]
							: [ ...actions.map((x) => x(item)), ..._actions ];
					return _actions;
				};
				listRenderItem = (item) => (
					<List.Item key={item.id} {...listItemProps}>
						<Card hoverable={true} {...cardProps} actions={getCardActions(item)}>
							{renderItem(item)}
						</Card>
					</List.Item>
				);
				break;
			case 'list':
				const getListItemActions = (item) => {
					let _actions = [];
					if (updateApi && PermissionWrapper.isGrant) {
						_actions.push(<Icon type="edit" onClick={() => this.showEditModal(item)} />);
					}
					if (updateApi && PermissionWrapper.isGrant) {
						_actions.push(<Icon type="delete" onClick={() => this.delete(item.id)} />);
					}
					_actions =
						actionLocation === 'right'
							? [ ..._actions, ...actions.map((x) => x(item)) ]
							: [ ...actions.map((x) => x(item)), ..._actions ];
					return _actions;
				};
				listRenderItem = (item) => (
					<List.Item key={item.id} {...listItemProps} actions={getListItemActions}>
						{renderItem(item)}
					</List.Item>
				);
				break;
		}

		return (
			<div>
				{(filterProps.filters || filterProps.advancedFilters) && (
					<Filter {...filterProps} onSearch={(value) => this.onFilter(value)} />
				)}

				<div>
					{perToolButtons}
					{createApi ? (
						<PermissionWrapper requiredPermission={createPermission}>
							<Button type="primary" style={this.buttonStyle} onClick={() => this.showAddModal()}>
								添加
							</Button>
						</PermissionWrapper>
					) : null}
					{deleteBatchApi ? (
						<PermissionWrapper requiredPermission={deletePermission}>
							<Button type="primary" style={this.buttonStyle} onClick={() => this.deleteBatch()}>
								批量删除
							</Button>
						</PermissionWrapper>
					) : null}
					{toExcelApi ? (
						<PermissionWrapper requiredPermission={toExcelPermission}>
							<Button style={this.buttonStyle} onClick={() => this.toExcel()}>
								导出到EXCEL
							</Button>
						</PermissionWrapper>
					) : null}
					{postToolButtons}
				</div>
				<List
					loading={loading}
					grid={{
						gutter: 16,
						xs: 1,
						sm: 1,
						md: 2,
						lg: 2,
						xl: 3,
						xxl: 4
					}}
					renderItem={listRenderItem}
					{...listProps}
					pagination={
						pagination && {
							...pagination,
							...paginationProps,
							onChange: (page, pageSize) => {
								this.onPageChange(page, pageSize);
							},
							onShowSizeChange: (page, pageSize) => {
								this.onPageChange(1, pageSize);
							}
						}
					}
					dataSource={items}
				/>
				<Modal
					maskClosable={false}
					visible={modalVisible}
					title={modalText}
					onCancel={() => {
						if (this.props.isCancel) {
							this.props.isCancel();
						}
						this.setState({ modalVisible: false });
					}}
					footer={[
						<Button key="save" loading={saveLoading} type="primary" onClick={() => this.save()}>
							保存
						</Button>,
						<Button
							key="cancel"
							onClick={() => {
								this.setState({ modalVisible: false });
							}}
						>
							取消
						</Button>
					]}
					{...modalProps}
				>
					{modalVisible ? renderFormNode(record, form.getFieldDecorator) : null}
				</Modal>
			</div>
		);
	}
}
