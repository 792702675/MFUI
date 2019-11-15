import React from 'react';
import { Popover } from 'antd';
import { PopoverProps } from 'antd/lib/popover';

export default class PopoverClickClose extends React.Component<PopoverProps, {}> {
	state = {
		visible: false
	};

	hide = () => {
		this.setState({
			visible: false
		});
	};

	handleVisibleChange = (visible) => {
		if (visible) {
			this.setState({ visible });
		}
	};

	render() {
		const { content, ...otherProps } = this.props;
		return (
			<Popover
				{...otherProps}
				content={
					<div>
						{content}
						<div style={{ textAlign: 'center' }}>
							<a onClick={this.hide}>关闭</a>
						</div>
					</div>
				}
				trigger="click"
				visible={this.state.visible}
				onVisibleChange={this.handleVisibleChange}
			/>
		);
	}
}
