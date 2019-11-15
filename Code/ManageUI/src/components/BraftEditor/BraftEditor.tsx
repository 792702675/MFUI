import React from 'react';
import BraftEditor from 'braft-editor';
import 'braft-editor/dist/index.css';
import { ContentUtils } from 'braft-utils';
import FilePickerModal from './FilePickerModal';
import Filter from '../Filter/Filter';
import * as api from '../../api/api';
import { call } from '../../api/apiUtil';
import { remoteUrl } from '@/utils/url';
import { message } from 'antd';
import reqwest from 'reqwest';

export default class Editor extends React.Component {
	constructor(extension) {
		super();

		BraftEditor.use(extension);

		this.state = {
			editorState: null,
			showPhotoView: false,
			tagOptions: [],

			pagination: {
				current: 1,
				pageSize: 27
			},
			selected: [],

			paginationArticle: {
				current: 1,
				pageSize: 27
			},
			selectedArticle: [],
			confirmLoading: false
		};
		this.hideModal = this.hideModal.bind(this);

		this.pageChange = this.pageChange.bind(this);
		this.getData = this.getData.bind(this);
		this.filter = this.filter.bind(this);

		this.pageChangeArticle = this.pageChangeArticle.bind(this);
		this.getDataArticle = this.getDataArticle.bind(this);
	}
	componentDidMount() {
		let editorState = BraftEditor.createEditorState(this.props.value && this.props.value.content, {
			editorId: this.props.editorId,
			...this.props.createEditorStateOption
		});
		this.setState({
			editorState
		});

		if (this.props.form && this.props.onChange) {
			setTimeout(() => {
				this.props.onChange({ ...this.props.value, __isEditor: true });
			});
		}

		if (this.props.fullscreen) {
			this.editor.toggleFullscreen(this.props.fullscreen);
		}
	}
	componentWillReceiveProps(nextProps) {
		if (this.props.fullscreen) {
			this.editor.toggleFullscreen(this.props.fullscreen);
		}
	}

	componentDidUpdate(prevProps, prevState) {
		if (prevProps.fullscreen != this.props.fullscreen) {
			this.editor.toggleFullscreen(prevProps.fullscreen);
		}
	}

	hideModal() {
		this.setState({ showPhotoView: false });
	}

	pageChange(num) {
		this.getData({ ...this.state.pagination, current: num }, this.state.filter);
	}

	pageChangeArticle(num) {
		this.getDataArticle({ ...this.state.paginationArticle, current: num }, { group: this.props.value.group });
	}

	filter(filter) {
		this.getData({ ...this.state.pagination, current: 1 }, filter);
	}

	filterArticle(filter) {
		this.getDataArticle({ ...this.state.paginationArticle, current: 1 }, filter);
	}

	getData(pagination, filter) {
		call({
			method: new api.OSSObjectApi().getAll,
			payload: {
				...filter,
				skipCount: (pagination.current - 1) * pagination.pageSize,
				maxResultCount: pagination.pageSize
			}
		}).then(({ result }) => {
			pagination = { ...pagination, total: result.totalCount };
			filter = { ...filter };
			this.update(
				result.items.map((x) => {
					return { id: x.eTag, thumbUrl: x.icon, ...x };
				}),
				pagination
			);
			this.setState({ pagination, filter });
		});
	}

	getDataArticle(paginationArticle, filterArticle) {
		call({
			method: new api.OSSObjectApi().getAll,
			payload: {
				...filterArticle,
				skipCount: (paginationArticle.current - 1) * paginationArticle.pageSize,
				maxResultCount: paginationArticle.pageSize
			}
		}).then(({ result }) => {
			paginationArticle = { ...paginationArticle, total: result.totalCount };
			filterArticle = { ...filterArticle };
			this.updateArticle(
				result.items.map((x) => {
					return { id: x.eTag, thumbUrl: x.icon, ...x };
				}),
				paginationArticle
			);
			this.setState({ paginationArticle, filterArticle });
		});
	}

	handleEditorChange = (editorState) => {
		this.setState({ editorState });
		let value = {
			group: this.props.value && this.props.value.group,
			htmlContent: this.state.editorState.toHTML(),
			content: this.state.editorState.toRAW()
		};
		if (this.props.onChange) {
			this.props.onChange(value);
		}
	};

	uploadFile = (formData, uploadFn) => {
		const { value = {}, onChange = () => {} } = this.props;
		this.setState({ confirmLoading: true });
		reqwest({
			url: value.group
				? `${remoteUrl}/api/OssApp/CreateObjectRelatedToThisArticle?group=${value.group}`
				: `${remoteUrl}/api/OssApp/CreateObjectRelatedToThisArticle`,
			method: 'post',
			processData: false,
			data: formData,
			success: (result) => {
				this.setState({ confirmLoading: false });
				onChange({ ...value, group: result.result.group });
				if (this.props.form) {
					let formValue = this.props.form.getFieldsValue();
					for (let name in formValue) {
						if (formValue[name] && formValue[name].__isEditor) {
							this.props.form.setFieldsValue({
								[name]: { ...formValue[name], group: result.result.group }
							});
						}
					}
				}

				if (uploadFn) {
					uploadFn.success({
						url: result.result.fileInfo[0].url
					});
				} else {
					this.setState({
						selectedArticle: result.result.fileInfo
					});
					this.filterArticle({ group: result.result.group });
				}
				message.success('上传成功');
			},
			error: () => {
				this.setState({ confirmLoading: false });
				message.error('上传失败');
			}
		});
	};

	uploadFn = (param) => {
		const formData = new FormData();
		formData.append('file', param.file);
		this.uploadFile(formData, param);
	};

	render() {
		const { editorState, showPhotoView, tagOptions, selected, selectedArticle, confirmLoading } = this.state;

		const {
			value = {},
			onChange = () => {},
			filePicker,
			extendControls = [],
			editorId,
			...otherProps
		} = this.props;

		const {
			multiple = true,
			enableTags = true,
			tags = [],
			enableExtensionNames = true,
			extensionNames = [],
			...otherPickerProps
		} =
			this.props.filePicker || {};

		let filter = [
			{
				name: 'name',
				displayName: '名称'
			}
		];
		if (enableTags) {
			filter.push({
				name: 'tagNames',
				displayName: '标签',
				type: 'select',
				selectOptions: tagOptions.map((x) => {
					return { name: x, value: x };
				}),
				value: tags,
				props: { mode: 'tags' }
			});
		}
		if (enableExtensionNames) {
			filter.push({
				name: 'extensionNames',
				displayName: '后缀名',
				type: 'select',
				selectOptions: [],
				value: extensionNames,
				props: { mode: 'tags' }
			});
		}

		return (
			<div>
				<BraftEditor
					contentStyle={{ height: '100px' }}
					{...otherProps}
					id={editorId}
					ref={(editor) => (this.editor = editor)}
					value={editorState}
					onChange={this.handleEditorChange}
					onSave={() => {}}
					// controls={BraftEditor.defaultProps.controls.filter((x) => x != 'media')}
					media={{ uploadFn: this.uploadFn }}
					extendControls={[
						{
							key: 'media-modal',
							type: 'button',
							text: '资源选择',
							onClick: () => {
								this.setState({
									showPhotoView: true,
									selected: [],
									selectedArticle: []
								});

								call({
									method: new api.TagApi().getAll
								}).then(({ result }) => {
									this.setState({ tagOptions: result });
								});

								this.getData(this.state.pagination, {
									tags: this.props.tags,
									extensionNames: this.props.extensionNames
								});

								if (this.props.value && this.props.value.group) {
									this.getDataArticle(this.state.paginationArticle, {
										group: this.props.value.group
									});
								} else {
									this.updateArticle([], this.state.paginationArticle);
								}
							}
						},
						...extendControls
					]}
				/>

				<FilePickerModal
					title="资源选择"
					multiple={multiple}
					{...otherPickerProps}
					visible={showPhotoView}
					onOk={(files) => {
						let _editorState = editorState;
						_editorState = ContentUtils.insertMedias(
							_editorState,
							files.map((x) => {
								let media = { type: 'EMBED' };
								switch (x.url.split('.').pop().toLowerCase()) {
									case 'jpg':
									case 'png':
									case 'bmp':
									case 'gif':
									case 'svg':
									case 'ico':
									case 'icon':
										media.type = 'IMAGE';

										break;
									case 'mp3':
									case 'wav':
									case 'ogg':
										media.type = 'AUDIO';
										break;
									case 'mp4':
									case 'webm':
										media.type = 'VIDEO';
										media.width = '100%';
										break;
								}
								return {
									url: x.url,
									...media
								};
							})
						);
						this.setState({
							editorState: _editorState,
							selected: [],
							selectedArticle: []
						});
						this.hideModal();
					}}
					onCancel={this.hideModal}
					fetch={(cb) => {
						this.update = cb;
					}}
					onPageChange={(num) => {
						this.pageChange(num);
					}}
					selected={selected}
					filter={<Filter searchProvide="nameVaule" filters={filter} onSearch={(v) => this.filter(v)} />}
					fetchArticle={(cb) => {
						this.updateArticle = cb;
					}}
					onPageChangeArticle={(num) => {
						this.pageChangeArticle(num);
					}}
					selectedArticle={selectedArticle}
					// uploadUrlArticle={
					// 	value.group ? (
					// 		`${remoteUrl}/api/OssApp/CreateObjectRelatedToThisArticle?group=${value.group}`
					// 	) : (
					// 		`${remoteUrl}/api/OssApp/CreateObjectRelatedToThisArticle`
					// 	)
					// }
					// uploadChangeArticle={(response) => {
					// 	if (response && response.success) {
					// 		onChange({ ...value, group: response.result.group });
					// 		if (this.props.form) {
					// 			let formValue = this.props.form.getFieldsValue();
					// 			for (let name in formValue) {
					// 				if (formValue[name] && formValue[name].__isEditor) {
					// 					this.props.form.setFieldsValue({
					// 						[name]: { ...formValue[name], group: response.result.group }
					// 					});
					// 				}
					// 			}
					// 		}

					// 		return {
					// 			data: {
					// 				id: response.result.fileInfo[0].eTag,
					// 				thumbUrl: response.result.fileInfo[0].icon,
					// 				...response.result.fileInfo[0]
					// 			},
					// 			success: true
					// 		};
					// 	} else {
					// 		return { success: false };
					// 	}
					// }}
					beforeUploadArticle={(file, fileList) => {
						if (file.uid != fileList[0].uid) {
							return false;
						}
						const formData = new FormData();
						fileList.forEach((file) => {
							formData.append('files[]', file);
						});

						this.uploadFile(formData);

						return false;
					}}
					confirmLoading={confirmLoading}
				/>
			</div>
		);
	}
}
