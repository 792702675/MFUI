import React from 'react';
import { Checkbox } from 'antd';
import { call } from '../../api/apiUtil';

export default class CheckboxGroup extends React.Component {
	constructor() {
		super();
		this.state = {
			options: []
		};
	}
	componentDidMount() {
		call({
			method: this.props.api,
			payload: this.props.payload
		}).then(({ result }) => {
			this.setState({
				options: result.map((x) => {
					return { label: x.name, value: x.value };
				})
			});
		});
	}
	render() {
		const { onChange, value, ...otherProps } = this.props;
		return (
			<Checkbox.Group
				options={this.state.options}
				{...otherProps}
				value={value}
				onChange={(value) => onChange(value)}
			/>
		);
	}
}
