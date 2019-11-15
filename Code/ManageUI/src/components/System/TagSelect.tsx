import React from 'react';
import { Select } from 'antd';
import * as api from '../../api/api';
import { call } from '../../api/apiUtil';

export default class TagSelect extends React.Component {
	constructor() {
		super();
		this.state = {
			tagOptions: []
		};
	}
	componentDidMount() {
		call({
			method:
				this.props.isSystem === true
					? new api.TagApi().getAllSystemTag
					: this.props.isSystem === false ? new api.TagApi().getAllNotSystemTag : new api.TagApi().getAll
		}).then(({ result }) => {
			this.setState({ tagOptions: result });
		});
	}
	render() {
		const { onChange, value, tagOptionsprops, ...otherProps } = this.props;
		return (
			<Select
				style={{ width: '100%' }}
				mode="tags"
				{...otherProps}
				value={value}
				onChange={(value) => onChange(value)}
			>
				{tagOptionsprops?
					tagOptionsprops.map((x, i) => {
						return (
							<Select.Option key={i} value={x}>
								{x}
							</Select.Option>
						);
					})
				:this.state.tagOptions.map((x, i) => {
					return (
						<Select.Option key={i} value={x}>
							{x}
						</Select.Option>
					);
				})}
			</Select>
		);
	}
}
