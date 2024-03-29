import React from 'react';
import { Form, Checkbox, Input, DatePicker, InputNumber, Row, Col, Button, Select, Icon, Cascader, Slider } from 'antd';
import moment from 'moment';
import EnumSelect from '../System/EnumSelect';
import CheckboxGroup from '../System/CheckboxGroup';
import CascaderSelect from '../System/CascaderSelect';
const create = Form.create;
const FormItem = Form.Item;
const Search = Input.Search;
const Option = Select.Option;
const RangePicker = DatePicker.RangePicker;

class Filter extends React.Component {
	static defaultProps = {
		searchProvide: 'nameVaule',
		searchName: 'filter'
	};
	state = {
		expand:
			this.props.advancedFilters &&
			this.props.advancedFilters.length &&
			(!this.props.filters || !this.props.filters.length)
	};

	componentDidUpdate(prevProps, prevState) {
		if (prevProps.expand != this.props.expand) {
			this.setState({
				expand: prevProps.expand
			});
		}
	}

	render() {
		const { form, filters = [], searchName, advancedFilters = [], searchProvide } = this.props;
		const useAdvancedSearch = advancedFilters.length > 0;
		const useAdvancedSearchOnly = advancedFilters && advancedFilters.length && (!filters || !filters.length);
		const { expand } = this.state;
		const { getFieldDecorator } = form;
		function option2name(option) {
			switch (option) {
				case '>':
					return 'gt';
				case '>=':
					return 'ge';
				case '<':
					return 'lt';
				case '<=':
					return 'le';
				case '==':
					return 'eq';
				case '!=':
					return 'ne';
				default:
					return option;
			}
		}

		function mapInit(n) {
			if (!n.type) {
				n.type = 'string';
			}
			if (!n.option) {
				if (n.type == 'string') {
					n.option = 'like';
				} else {
					n.option = '==';
				}
			}
		}
		filters.map((n) => {
			if (n.name) {
				mapInit(n);
				n.fileldName = option2name(n.option) + '_1_' + n.name;
			}
		});
		advancedFilters.map((n) => {
			if (n.name) {
				mapInit(n);
				n.fileldName = option2name(n.option) + '_2_' + n.name;
			}
		});

		const advancedButtonStyle = {
			fontFamily: 'PingFangSC-Regular',
			fontSize: '12px',
			color: '#537FDF',
			letterSpacing: 0,
			lineHeight: '12px',
			position: 'absolute',
			right: '12px'
		};
		const advancedSearch = useAdvancedSearch ? (
			<a
				style={advancedButtonStyle}
				onClick={() => {
					this.setState({
						expand: true
					});
				}}
			>
				{useAdvancedSearchOnly ? null : (
					<span>
						高级搜索 <Icon type="down" />
					</span>
				)}
			</a>
		) : null;
		const _this = this;

		function buildSql(prop, value, sql) {
			if (!prop) return sql;
			if (prop.type == 'cascader') {
				if (value.length) {
					let keys = prop.name.split(',');
					for (let i = 0; i < value.length; i++) {
						sql = `${sql} ${keys[i]} == ${value[i]} &`;
					}
				}
			} else if (prop.type == 'datetimerange') {
				if (value.length) {
					var start = moment(value[0].format('YYYY-MM-DD')).format(
						prop.format ? prop.format : 'YYYY-MM-DD HH:mm:ss'
					);
					var end = moment(value[1].format('YYYY-MM-DD'))
						.add(24 * 60 * 60 - 1, 'seconds')
						.format(prop.format ? prop.format : 'YYYY-MM-DD HH:mm:ss');
					sql = `${sql} ${prop.name} >= ${start} &`;
					sql = `${sql} ${prop.name} <= ${end} &`;
				}
			} else {
				value =
					prop.type == 'datetime' ? moment(value).format(prop.format ? prop.format : 'YYYY-MM-DD') : value;
				sql = `${sql} ${prop.name} ${prop.option} ${value} &`;
			}
			return sql;
		}
		function onSearch(values) {
			if (searchProvide == 'nameVaule') {
				var result = {};
				for (var name in values) {
					if (values[name]) {
						var prop = [ ...filters, ...advancedFilters ].filter((n) => n.fileldName == name)[0];
						var value =
							prop.type == 'datetime'
								? moment(values[name]).format(prop.format ? prop.format : 'YYYY-MM-DD')
								: values[name];
						if (prop.type == 'cascader') {
							if (value.length) {
								let keys = prop.name.split(',');
								for (let i = 0; i < value.length; i++) {
									result[keys[i]] = value[i];
								}
							}
							continue;
						}
						result[prop.name || searchName] = value;
					}
				}
				_this.props.onSearch(result);
			} else if (searchProvide == 'sql') {
				let sql = '';
				for (var name in values) {
					if (values[name]) {
						var prop = [ ...filters, ...advancedFilters ].filter((n) => n.fileldName == name)[0];
						let value = values[name];
						if (!prop && typeof values[name] == 'object') {
							for (let i in filters) {
								let v = filters[i];
								if (v.fileldName.indexOf(name + '.') == 0) {
									var names = v.fileldName.split('.');
									value = values[names[0]];
									for (let i = 1; i < names.length; i++) {
										if (!value) break;
										value = value[names[i]];
									}
									if (value) {
										sql = buildSql(v, value, sql);
									}
								}
							}
							for (let i in advancedFilters) {
								let v = advancedFilters[i];
								if (v.fileldName.indexOf(name + '.') == 0) {
									var names = v.fileldName.split('.');
									value = values[names[0]];
									for (let i = 1; i < names.length; i++) {
										if (!value) break;
										value = value[names[i]];
									}
									if (value) {
										sql = buildSql(v, value, sql);
									}
								}
							}
						} else {
							sql = buildSql(prop, value, sql);
						}
					}
				}
				sql = sql.replace(/&$/, '');
				let result = {};
				result[searchName] = sql;
				_this.props.onSearch(result);
			} else if (typeof searchProvide === 'function') {
				_this.props.onSearch(searchProvide(values));
			} else {
				_this.props.onSearch(values);
			}
		}

		const formItemProps = {
			style: {
				marginBottom: '12px'
			}
		};
		const mapField = (n, i) => (
			<span key={i}>
				{((n) => {
					switch (n.type) {
						case 'boolean':
							return (
								<FormItem help={n.description} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										valuePropName: 'checked',
										rules: n.rules
									})(<Checkbox {...n.props}>{n.displayName}</Checkbox>)}
								</FormItem>
							);
						case 'datetime':
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(<DatePicker placeholder="" {...n.props} />)}
								</FormItem>
							);
						case 'datetimerange':
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(<RangePicker {...n.props} />)}
								</FormItem>
							);
						case 'number':
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(<InputNumber {...n.props} />)}
								</FormItem>
							);
						case 'select':
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(
										<Select style={{ minWidth: '161px' }} allowClear={true} {...n.props}>
											{n.selectOptions.map((o, i) => (
												<Option key={i} value={o.value}>
													{o.name}
												</Option>
											))}
										</Select>
									)}
								</FormItem>
							);
						case 'enumSelect':
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(<EnumSelect style={{ minWidth: '161px' }} {...n.props} />)}
								</FormItem>
							);
						case 'checkboxGroup':
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(<CheckboxGroup {...n.props} />)}
								</FormItem>
							);
						case 'cascader':
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(
										<Cascader
											placeholder=""
											style={{ minWidth: '161px' }}
											allowClear={true}
											expandTrigger="hover"
											changeOnSelect={true}
											{...n.props}
										/>
									)}
								</FormItem>
							);
						case 'cascaderSelect':
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(
										<CascaderSelect
											placeholder=""
											style={{ minWidth: '161px' }}
											allowClear={true}
											expandTrigger="hover"
											changeOnSelect={true}
											{...n.props}
										/>
									)}
								</FormItem>
							);
						case 'slider':
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(<Slider style={{ width: '161px' }} {...n.props} />)}
								</FormItem>
							);
						case 'br':
							return <br />;
						case 'custom':
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(n.component)}
								</FormItem>
							);
						default:
							return (
								<FormItem help={n.description} label={n.displayName} {...formItemProps}>
									{getFieldDecorator(n.fileldName, {
										initialValue: n.value,
										rules: n.rules
									})(<Input {...n.props} />)}
								</FormItem>
							);
					}
				})(n)}
			</span>
		);
		return !expand ? (
			<div>
				{filters.length == 0 ? null : filters.length == 1 &&
				(filters[0].type == undefined || filters[0].type == 'string') ? (
					<div>
						<Row>
							<Col span={8}>
								<Search
									enterButton
									placeholder={filters[0].displayName}
									onSearch={(value) => {
										var values = {};
										values[filters[0].fileldName] = value;
										onSearch(values);
									}}
								/>
							</Col>
							{advancedSearch}
						</Row>
						<br />
					</div>
				) : (
					<div style={{ position: 'relative' }}>
						<Form layout="inline">
							{filters.map(mapField)}
							<FormItem>
								<Button
									type="primary"
									onClick={() => {
										form.validateFields((err, values) => {
											onSearch(values);
										});
									}}
								>
									查询
								</Button>
								<Button
									style={{ marginLeft: '12px' }}
									onClick={() => {
										form.resetFields();
										this.props.onFilterClear && this.props.onFilterClear();
									}}
								>
									清除条件
								</Button>
							</FormItem>
							{advancedSearch}
						</Form>
					</div>
				)}
			</div>
		) : (
			<div>
				<Form
					layout="inline"
					style={{
						position: 'relative',
						background: '#F3F5F9',
						border: '1px solid #E6EAF1',
						borderRadius: '4px',
						padding: '16px'
					}}
				>
					{advancedFilters.map(mapField)}
					<br />
					<Row gutter={24} type="flex" align="middle" style={{ marginTop: 12 }}>
						<Col span={16} offset={1}>
							<Button
								type="primary"
								onClick={() => {
									form.validateFields((err, values) => {
										onSearch(values);
									});
								}}
							>
								查询
							</Button>
							<Button
								style={{ marginLeft: '12px' }}
								onClick={() => {
									form.resetFields();
									this.props.onFilterClear && this.props.onFilterClear();
								}}
							>
								清除条件
							</Button>
						</Col>
						{useAdvancedSearchOnly ? null : (
							<a
								style={advancedButtonStyle}
								onClick={() => {
									this.setState({ expand: false });
								}}
							>
								收起高级搜索 <Icon type="up" />
							</a>
						)}
					</Row>
				</Form>
				<br />
			</div>
		);
	}
}

export default create()(Filter);
