import React from 'react';
import { Select } from 'antd';
import { call } from '../../api/apiUtil';
import 'pinyin4js';

export default class EnumSelect extends React.Component {
	constructor() {
		super();
		this.state = {
			options: []
		};
	}
	componentDidMount() {
		if (this.props.api) {
			call({
				method: this.props.api,
				payload: this.props.payload
			}).then(({ result }) => {
				this.setState({ options: this.props.apiResultMap ? result.map(this.props.apiResultMap) : result });
				if (this.props.onApiLoad) {
					this.props.onApiLoad(result);
				}
			});
		}
	}
	render() {
		const {
			onChange,
			value,
			optionRender = (x, i) => (
				<Select.Option {...x} key={i} value={x.value}>
					{x.name}
				</Select.Option>
			),
			...otherProps
		} = this.props;
		return (
			<Select
				style={{ width: '100%' }}
				showSearch
				filterOption={(input, option) =>
					option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0 ||
					PinyinHelper.convertToPinyinString(
						option.props.children.toLowerCase(),
						'',
						PinyinFormat.FIRST_LETTER
					)
						.toString()
						.replace(/,/g, '')
						.indexOf(input.toLowerCase()) >= 0 ||
					PinyinHelper.convertToPinyinString(
						option.props.children.toLowerCase(),
						'',
						PinyinFormat.WITHOUT_TONE
					)
						.toString()
						.replace(/,/g, '')
						.indexOf(input.toLowerCase()) >= 0}
				{...otherProps}
				value={value}
				onChange={(value, option) => onChange(value, option)}
			>
				{this.state.options.map((x, i) => {
					return optionRender(x, i);
				})}
			</Select>
		);
	}
}
