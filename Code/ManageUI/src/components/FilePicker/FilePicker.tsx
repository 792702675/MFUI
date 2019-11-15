import React from 'react';
import { connect } from 'dva';
import { Button, Upload } from 'antd';
import ImgPickerModal from './FilePickerModal';
import Filter from '../Filter/Filter';
import GroupTagSelect from '../System/GroupTagSelect';
import { remoteUrl } from '../../utils/url';

import UploadList from 'antd/lib/upload/UploadList';
import { previewImage, isImageUrl } from 'antd/lib/upload/utils';
import defaultLocale from 'antd/lib/locale-provider/default';

import * as api from '../../api/api';
import { call } from '../../api/apiUtil';
import { JsonHubProtocol } from '@aspnet/signalr';

class FilePicker extends React.Component {
	constructor() {
		super();
		this.state = {
			showPhotoView: false,
			pagination: {
				current: 1,
				pageSize: 27
			},
			tagOptions: []
		};
		this.hideModal = this.hideModal.bind(this);
		this.pageChange = this.pageChange.bind(this);
		this.getData = this.getData.bind(this);
		this.filter = this.filter.bind(this);
	}

	componentDidMount() {
		this.getData(this.state.pagination, { tags: this.props.tags, extensionNames: this.props.extensionNames });
		call({
			method: new api.TagApi().getAll
		}).then(({ result }) => {
			this.setState({ tagOptions: result });
		});
	}

	componentDidUpdate(prevProps, prevState) {
		if (
			JSON.stringify(prevProps.tags) != JSON.stringify(this.props.tags) ||
			JSON.stringify(prevProps.extensionNames) != JSON.stringify(this.props.extensionNames)
		) {
			this.getData(this.state.pagination, { tags: this.props.tags, extensionNames: this.props.extensionNames });
		}
	}

	hideModal() {
		this.setState({ showPhotoView: false });
	}

	pageChange(num) {
		this.getData({ ...this.state.pagination, current: num }, this.state.filter);
	}

	filter(filter) {
		this.getData({ ...this.state.pagination, current: 1 }, filter);
	}

	getData(pagination, filter) {
		call({
			method: new api.OSSObjectApi().getAll,
			payload: {
				...filter,
				skipCount: (pagination.current - 1) * pagination.pageSize,
				maxResultCount: pagination.pageSize,
				sysFunName: this.props.sysFunName
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

	render() {
		const { showPhotoView, tagOptions } = this.state;

		const {
			value,
			onChange,
			type = 'image',
			multiple = false,
			enableTags = true,
			notSee = false,
			tags = [],
			enableExtensionNames = true,
			extensionNames = [],
			disabled = false,
			...otherProps
		} = this.props;

		let filter = [
			{
				name: 'name',
				displayName: '名称'
			}
		];
		if (enableTags) {
			filter.push({
				type: 'br'
			});
			filter.push({
				name: 'tagNames',
				displayName: '标签',
				value: tags,
				type: 'custom',
				component: <GroupTagSelect />
			});
		}
		if (enableExtensionNames) {
			filter.push({
				type: 'br'
			});
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
				<Button
					disabled={disabled}
					type="primary"
					onClick={() => {
						this.setState({ showPhotoView: true });
					}}
				>
					{type == 'image' ? '选择图片' : '选择文件'}
				</Button>
				{notSee ? null : (
					<UploadList
						listType={type == 'image' ? 'picture' : 'text'}
						items={(value ? (multiple ? value : [ value ]) : [])
							.map((x) => {
								return { url: x.url, uid: x.eTag, name: x.url.substring(x.url.lastIndexOf('/') + 1) };
							})
							.filter((x) => x.url)}
						previewFile={previewImage}
						onRemove={(file) => {
							onChange(multiple ? value.filter((x) => x.eTag != file.uid) : null);
						}}
						showRemoveIcon={!disabled}
						showPreviewIcon={true}
						locale={defaultLocale.Upload}
					/>
				)}

				<ImgPickerModal
					title="资源选择"
					multiple={multiple}
					{...otherProps}
					visible={showPhotoView}
					fetch={(cb) => {
						this.update = cb;
					}}
					onOk={(files) => {
						onChange(multiple ? files : files[0]);
						this.hideModal();
					}}
					onPageChange={(num) => {
						this.pageChange(num);
					}}
					onCancel={this.hideModal}
					onChange={(file) => {
						// console.log(file);
					}}
					selected={value ? multiple ? value : [ value ] : []}
					filter={
						<Filter
							searchProvide="nameVaule"
							filters={[
								{
									name: 'name',
									displayName: '名称'
								}
							]}
							advancedFilters={filter}
							onSearch={(v) => this.filter(v)}
						/>
					}
				/>
			</div>
		);
	}
}

export default FilePicker;
