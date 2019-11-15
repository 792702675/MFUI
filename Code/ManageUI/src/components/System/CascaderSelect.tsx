import React from 'react';
import { Cascader } from 'antd';
import { call } from '../../api/apiUtil';
import 'pinyin4js';

export default class CascaderSelect extends React.Component {
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
			this.setState({ options: result });
		});
	}
	render() {
		const { onChange, value, islastLevel = false, ...otherProps } = this.props;

		function eachValue(options, value) {
			for (let o of options) {
				if (o.children) {
					let result = eachValue(o.children, value);
					if (result) {
						return [ o.value, ...result ];
					}
				} else {
					if (o.value == value) {
						return [ o.value ];
					}
				}
			}
		}
		let _value = eachValue(this.state.options, value);
		return (
			<Cascader
				style={{ width: '100%' }}
				placeholder=""
				options={this.state.options}
				{...otherProps}
				value={islastLevel ? _value : value}
				onChange={(value) => onChange(islastLevel ? value[value.length - 1] : value)}
			/>
		);
	}
}
