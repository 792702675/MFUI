import React from 'react';

import { Tag, Icon, Select, Tooltip } from 'antd';

export default class TagGroupSelect extends React.Component {
	state = {
		inputVisible: false
	};

	handleClose = (removedTag, index) => {
		let newValue = [ ...this.props.value ];
		console.log(newValue)
		newValue.splice(index, 1);
		console.log(newValue)

		this.props.onChange(newValue);
	};

	showInput = () => {
		this.setState({ inputVisible: true }, () => this.input.focus());
	};

	handleChange = (selectValue) => {
		let value = this.props.value || [];
		// if (selectValue && value.indexOf(selectValue) === -1) {
		value = [ ...value, selectValue ];
		// }
		this.setState({
			inputVisible: false
		});
		this.props.onChange(value);
	};

	handleBlur = () => {
		this.setState({
			inputVisible: false
		});
	};

	saveInputRef = (input) => (this.input = input);

	render() {
		const { inputVisible } = this.state;
		const { value = [], selectOptions = [] } = this.props;
		return (
			<div>
				{value.map((tag, index) => {
					const isLongTag = tag.length > 20;
					const tagElem = (
						<Tag key={index} closable={true} onClose={() => this.handleClose(tag, index)}>
							{isLongTag ? `${tag.slice(0, 20)}...` : tag}
						</Tag>
					);
					return isLongTag ? (
						<Tooltip title={tag} key={tag}>
							{tagElem}
						</Tooltip>
					) : (
						tagElem
					);
				})}
				{inputVisible && (
					<Select
						ref={this.saveInputRef}
						size="small"
						style={{ width: 78 }}
						onChange={this.handleChange}
						onBlur={this.handleBlur}
					>
						{selectOptions.map((o, i) => (
							<Select.Option key={i} value={o.value}>
								{o.name}
							</Select.Option>
						))}
					</Select>
				)}
				{!inputVisible && (
					<Tag onClick={this.showInput} style={{ background: '#fff', borderStyle: 'dashed' }}>
						<Icon type="plus" />
					</Tag>
				)}
			</div>
		);
	}
}
