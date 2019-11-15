import React from 'react';

import { Tag, Icon, Input, Tooltip } from 'antd';

export default class TagGroup extends React.Component {
	state = {
		inputVisible: false,
		inputValue: ''
	};

	handleClose = (removedTag) => {
		const value = this.props.value.filter((tag) => tag !== removedTag);
		this.props.onChange(value);
	};

	showInput = () => {
		this.setState({ inputVisible: true }, () => this.input.focus());
	};

	handleInputChange = (e) => {
		this.setState({ inputValue: e.target.value });
	};

	handleInputConfirm = () => {
		const { inputValue } = this.state;
		let { value = [] } = this.props;
		if (inputValue && value.indexOf(inputValue) === -1) {
			value = [ ...value, inputValue ];
		}
		this.setState({
			inputVisible: false,
			inputValue: ''
		});
		this.props.onChange(value);
	};

	saveInputRef = (input) => (this.input = input);

	render() {
		const { inputVisible, inputValue } = this.state;
		const { value = [] } = this.props;
		return (
			<div>
				{value.map((tag, index) => {
					const isLongTag = tag.length > 20;
					const tagElem = (
						<Tag key={tag} closable={true} onClose={() => this.handleClose(tag)}>
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
					<Input
						ref={this.saveInputRef}
						type="text"
						size="small"
						style={{ width: 78 }}
						value={inputValue}
						onChange={this.handleInputChange}
						onBlur={this.handleInputConfirm}
						onPressEnter={this.handleInputConfirm}
					/>
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
