import React from 'react';
import { DatePicker, Select } from 'antd';
import moment from 'moment';

export default class Index extends React.Component {
	state = {
		startValue: null,
		endValue: null,
		endOpen: false,
		selectValue: null
	};

	componentDidUpdate(prevProps, prevState) {
		if (
			prevProps.value &&
			this.props.value &&
			(prevProps.value[0] != this.props.value[0] || prevProps.value[1] != this.props.value[1])
		) {
			this.setState({
				startValue: this.props.value[0] ? moment(this.props.value[0]) : null,
				endValue: this.props.value[1] ? moment(this.props.value[1]) : null
			});
		}
		if (!this.props.value && prevProps.value) {
			this.setState({
				startValue: null,
				endValue: null,
				selectValue: null
			});
		}
	}

	disabledStartDate = (startValue) => {
		const { endValue } = this.state;
		if (!startValue || !endValue) {
			return false;
		}
		return startValue.valueOf() > endValue.valueOf();
	};

	disabledEndDate = (endValue) => {
		const { startValue } = this.state;
		if (!endValue || !startValue) {
			return false;
		}
		return endValue.valueOf() <= startValue.valueOf();
	};

	onChange = (field, value) => {
		this.setState(
			{
				[field]: value
			},
			() => {
				if (this.props.onChange) {
					this.props.onChange([
						this.state.startValue ? this.state.startValue.format('YYYY-MM-DD 00:00:00') : null,
						this.state.endValue ? this.state.endValue.format('YYYY-MM-DD 23:59:59') : null
					]);
				}
			}
		);
	};

	onStartChange = (value) => {
		this.onChange('startValue', value);
		this.setState({
			selectValue: null
		});
	};

	onEndChange = (value) => {
		this.onChange('endValue', value);
		this.setState({
			selectValue: null
		});
	};

	handleStartOpenChange = (open) => {
		if (!open) {
			this.setState({ endOpen: true });
		}
	};

	handleEndOpenChange = (open) => {
		this.setState({ endOpen: open });
	};

	handleSelectChange = (value) => {
		this.setState(
			{
				startValue: value >= 0 ? moment().add(-value, 'days') : null,
				endValue: value >= 0 ? moment() : null,
				selectValue: value
			},
			() => {
				this.onChange('endOpen', false);
			}
		);
	};

	render() {
		const { startValue, endValue, endOpen, selectValue } = this.state;
		return (
			<div>
				<DatePicker
					disabledDate={this.disabledStartDate}
					format="YYYY-MM-DD"
					value={startValue}
					placeholder="开始日期"
					onChange={this.onStartChange}
					onOpenChange={this.handleStartOpenChange}
				/>
				<span> - </span>
				<DatePicker
					disabledDate={this.disabledEndDate}
					format="YYYY-MM-DD"
					value={endValue}
					placeholder="结束日期"
					onChange={this.onEndChange}
					open={endOpen}
					onOpenChange={this.handleEndOpenChange}
				/>
				<Select
					onChange={this.handleSelectChange}
					value={selectValue}
					allowClear
					placeholder="时间段"
					style={{ width: '150px', marginLeft: '16px' }}
				>
					<Select.Option value="-1">全部</Select.Option>
					<Select.Option value="0">今天截止</Select.Option>
					<Select.Option value="6">过去7天截止</Select.Option>
					<Select.Option value="29">过去30天截止</Select.Option>
				</Select>
			</div>
		);
	}
}
