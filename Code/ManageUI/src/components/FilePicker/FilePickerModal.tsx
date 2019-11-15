import React, { Component } from 'react';
import { Modal, Upload, Button, Icon, message, Spin, Pagination, Popover } from 'antd';
import PropTypes from 'prop-types';
import './style/index';

const noop = () => {};
// 减少了listparam，listUrl，selectedFileList, transformListData, beforeErrorHandler参数
class FilePickerModal extends Component {
	static propTypes = {
		// 选择部分
		fetch: PropTypes.func, // 展现列表获取逻辑
		selected: PropTypes.array, // 已选择图片,id list
		min: PropTypes.number, // 最少选择图片数量
		max: PropTypes.number, // 最多选择图片
		multiple: PropTypes.bool, // 是否允许选择多个图片
		onChange: PropTypes.func, // 图片数量变化时候调用
		onOk: PropTypes.func, // 弹窗选择确认回调函数
		// 基础显示部分
		limitInfo: PropTypes.string, // 超出文字，业务需要
		notice: PropTypes.string, // 备注信息
		// 上传部分
		uploadNotice: PropTypes.any, // 上传提示
		uploadUrl: PropTypes.string, // 上传图片的url
		uploadMulti: PropTypes.bool, // 是否允许选择多张图片上传
		uploadParams: PropTypes.object, // 上传附带参数
		beforeUpload: PropTypes.func, // 上传筛选函数,函数参数file: {type: string, size: number} ＝> bool false为有错误
		uploadChange: PropTypes.func, // 上传处理函数，服务器返回函数处理，需要返回{success: false, true. data: {id: string, thumbUrl: string, url: string}};
		chooseUpload: PropTypes.bool, // 勾选中上传的内容
		lazyLoad: PropTypes.bool
	};

	static defaultProps = {
		title: ' ',
		notice: '',
		fetch: noop,
		selected: [],
		min: 1,
		max: 9999,
		multiple: true,
		onChange: noop,
		limitInfo: '超出选择限制',
		onOk: noop,
		uploadUrl: '',
		uploadMulti: false,
		uploadParams: {},
		beforeUpload: noop,
		uploadChange: noop,
		chooseUpload: false,
		lazyLoad: false
	};

	constructor(props) {
		super(props);
		this.state = {
			visible: typeof this.props.visible === 'boolean' ? this.props.visible : true,
			data: [], // 图片数据
			selectedMap: this.props.selected.reduce((memo, row) => {
				return { ...memo, [row.eTag]: true };
			}, {}), // 选中内容
			selected: [], // 如果是分页选中需做保留处理
			pagination: {}, // 分页的情况
			fileList: [], // 上传的受限模式
			picLoading: true, // 初始化图片loading
			loading: false // 上传loading
		};
	}

	componentWillMount() {
		this.fetchData();
	}

	componentWillReceiveProps(nextProps) {
		if (nextProps.visible && this.props.visible !== nextProps.visible) {
			this.setState({
				visible: nextProps.visible
			});
		}
		if (nextProps.selected && JSON.stringify(this.props.selected) !== JSON.stringify(nextProps.selected)) {
			this.setState({
				selectedMap: nextProps.selected.reduce((memo, row) => {
					return { ...memo, [row.eTag]: true };
				}, {})
			});
		}
	}

	getSelected = () => {
		const { selectedMap = {}, data = [], selected } = this.state;
		const selectedIds = { ...selectedMap };
		const selectedFileList = [ ...data, ...selected ].filter((row) => {
			const choosed = selectedIds[row.id];
			delete selectedIds[row.id];
			return choosed;
		});
		return selectedFileList;
	};

	onOk = () => {
		this.props.onOk(this.getSelected());
		this.onCancel();
	};

	onCancel = () => {
		if (!this.props.onCancel) {
			this.setState({ visible: !this.state.visible });
		}
	};

	// to new Pic Position
	toNewPicView = () => {
		const picContainer = this.picContainer;
		if (picContainer) {
			picContainer.scrollTop = picContainer.scrollHeight;
		}
	};

	handleChange = (info) => {
		const fileList = info.fileList.filter((file) => {
			if (file.response) {
				const { data, success, silence } = this.props.uploadChange(file.response);
				if (!success) {
					if (!silence) {
						message.error('上传失败', 1);
					}
					this.setState({ loading: false });
				} else {
					if (!silence) {
						message.success('上传成功', 1);
					}

					if (this.props.chooseUpload) {
						this.selectPhoto(data && data.id, data);
					}

					const { pagination = {} } = this.state;
					this.setState(
						{
							loading: false,
							pagination: { ...this.state.pagination, current: 1 } // 上传完直接跳入第一页，需要服务端配合将图片放入第一页，个人感觉不理想
						},
						() => {
							if (pagination.total) {
								this.props.onPageChange(1);
							} else {
								this.toNewPicView();
							}
						}
					);
				}
				return false;
			}
			return true;
		});
		this.setState({
			fileList
		});
	};

	isPromise(obj) {
		return !!obj && (typeof obj === 'object' || typeof obj === 'function') && typeof obj.then === 'function';
	}

	beforeUpload = (file) => {
		let errorFlag = true;
		const { beforeUpload } = this.props;
		if (beforeUpload && typeof beforeUpload === 'function') {
			errorFlag = beforeUpload(file);
		} else if ([ 'image/jpeg', 'image/gif', 'image/png', 'image/bmp' ].indexOf(file.type) < 0) {
			message.error('图片格式错误');
			errorFlag = false;
		} else if (file.size > 20971520) {
			// 单个文件限制大小为20*1024*1024
			message.error('图片已超过20M');
			errorFlag = false;
		} else if (file.size < 204800) {
			// 单个文件限制大小为200*1024
			message.error('图片小于200kb');
			errorFlag = false;
		}

		if (errorFlag && !this.isPromise(errorFlag)) {
			// 如果是promise格式则不能判断loading状态
			this.setState({ loading: true });
		}
		return errorFlag;
	};

	selectPhoto = (id, data) => {
		const { multiple, max, limitInfo, onChange } = this.props;
		let { selectedMap } = this.state;
		const choosedSelect = multiple ? [ ...this.state.selected, data ] : [ data ];
		if (multiple) {
			if (selectedMap[id]) {
				delete selectedMap[id];
			} else if (Object.keys(selectedMap).length < max) {
				selectedMap[id] = true;
			} else if (limitInfo) {
				message.info(limitInfo);
			}
		} else {
			selectedMap = { [id]: true };
		}
		this.setState(
			{
				selected: choosedSelect,
				selectedMap
			},
			() => {
				if (onChange) {
					onChange(this.getSelected());
				}
			}
		);
	};

	fetchData = () => {
		this.props.fetch((data, pagination = {}) => {
			this.setState({
				picLoading: false,
				data,
				pagination
			});
		});
	};

	uploadProps = () => {
		const { uploadParams, uploadUrl, uploadMulti } = this.props;
		return {
			action: uploadUrl,
			withCredentials: true,
			data: uploadParams,
			onChange: this.handleChange,
			beforeUpload: this.beforeUpload,
			showUploadList: false,
			multiple: uploadMulti,
			fileList: this.state.fileList
		};
	};

	getPics = () => {
		const { data, selectedMap } = this.state;
		return data.map((row, i) => {
			return (
				<div
					key={i}
					className="hermes-photo-picker-list-item"
					onClick={this.selectPhoto.bind(this, row.id, row)}
					style={row.isImage ? {} : { verticalAlign: 'middle' }}
				>
					{row.isImage ? (
						<Popover
							content={
								<div style={{ textAlign: 'center' }}>
									<img src={row.url} alt="img" style={{ maxWidth: 640, maxHeight: 360 }} />
								</div>
							}
							title={row.name}
							mouseEnterDelay={1.5}
						>
							<div className="hermes-photo-picker-list-item-fileName">{row.name}</div>
							<div className="hermes-photo-picker-list-item-filesize">{row.width + '×' + row.height}</div>
							<img src={row.thumbUrl ? row.thumbUrl : row.url} alt="img" />
						</Popover>
					) : (
						[
							<div style={{ fontSize: '40px', lineHeight: '40px', marginBottom: '5px' }}>
								<Icon type={row.icon} />
							</div>,
							<div style={{ fontSize: '12px', lineHeight: '15px' }}>{row.name}</div>
						]
					)}
					{selectedMap[row.id] && (
						<div className="hermes-photo-picker-list-item-icon">
							<Icon type="check-circle" />
						</div>
					)}
				</div>
			);
		});
	};

	render() {
		const { multiple, min, uploadUrl, notice, onPageChange = noop, uploadNotice, filter } = this.props;
		const { data, loading, selectedMap, picLoading, visible, pagination } = this.state;
		const selectedCount = Object.keys(selectedMap).length;

		return (
			<div>
				<Modal
					visible={visible}
					width={912}
					zIndex={1000}
					footer=""
					onCancel={this.onCancel}
					maskClosable={false}
					{...this.props}
				>
					{filter ? filter : null}
					{uploadUrl ? (
						<div className="hermes-photo-picker-header">
							<div className="hermes-photo-picker-upload">
								<Upload {...this.uploadProps()}>
									{uploadNotice ? (
										<Popover placement="topRight" content={uploadNotice}>
											<Button size="large" type="ghost" loading={loading}>
												上传新图片
											</Button>
										</Popover>
									) : (
										<Button size="large" type="ghost" loading={loading}>
											上传新图片
										</Button>
									)}
								</Upload>
							</div>
						</div>
					) : null}
					{picLoading ? (
						<div style={{ textAlign: 'center', lineHeight: '280px' }}>
							<Spin />
						</div>
					) : (
						<div>
							{!(data && data.length) ? (
								<div style={{ textAlign: 'center', lineHeight: '280px' }}>暂无图片</div>
							) : (
								this.getPics()
							)}
						</div>
					)}
					<div className="hermes-photo-picker-footer">
						{notice ? <p className="notice-info">{notice}</p> : null}
						{pagination && pagination.total ? (
							<div className="hermes-photo-picker-pagination">
								<Pagination
									{...pagination}
									onChange={(num) => {
										this.setState(
											{
												picLoading: true,
												pagination: {
													...pagination,
													current: num
												}
											},
											() => {
												onPageChange(num);
											}
										);
									}}
								/>
							</div>
						) : null}
						<Button
							size="large"
							type="primary"
							onClick={this.onOk}
							disabled={selectedCount === 0 || (multiple && selectedCount < min)}
						>
							确定{multiple ? ` (${selectedCount})` : ''}
						</Button>
					</div>
				</Modal>
			</div>
		);
	}
}

export default FilePickerModal;
