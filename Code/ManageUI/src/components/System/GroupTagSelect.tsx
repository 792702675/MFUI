import React from 'react';
import { Select, Row, Col } from 'antd';
import * as api from '../../api/api';
import { call } from '../../api/apiUtil';
import TagSelect from './TagSelect';

export default class GroupTagSelect extends React.Component {
	constructor() {
		super();
		this.state = {
			groupTagOptions: []
		};
	}
	componentDidMount() {
		call({
			method: new api.SysFunApi().getGroupAndTagList
		}).then(({ result }) => {
			this.setState({ groupTagOptions: result });
		});
	}
	render() {
		const { onChange, value, tagSelectProps, style, ...otherProps } = this.props;
		return (
			<Row gutter={8} style={{ minWidth: '700px', ...style }}>
				<Col span={6}>
					<Select
						onChange={(x) => {
							onChange(this.state.groupTagOptions.find((y) => y.value == x).tags);
						}}
						{...otherProps}
					>
						{this.state.groupTagOptions.map((x, i) => {
							return (
								<Select.Option key={i} value={x.value}>
									{x.name}
								</Select.Option>
							);
						})}
					</Select>
				</Col>
				<Col span={18}>
					<TagSelect onChange={onChange} value={value} {...tagSelectProps} />
				</Col>
			</Row>
		);
	}
}
