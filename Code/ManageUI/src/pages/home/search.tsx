import React from 'react';
import { Table, Tabs, Input, Select, Form, Button } from 'antd';
const { TabPane } = Tabs;
const { Search } = Input;
const { Option } = Select;
import { call } from '../../api/apiUtil';
import * as api from '../../api/api';

export default class search extends React.Component {
	constructor() {
		super();
		this.state = {
			data: [],
			loading: false,
			selsectValue: [ 'Name' ],
			advancedValue: [
				{ occur: 'MUST', field: 'Name', name: '名称', keyword: '' },
				{ occur: 'MUST', field: 'Author', name: '作者', keyword: '' },
				{ occur: 'MUST', field: 'Content', name: '内容', keyword: '' }
			]
		};
	}
	componentDidMount() {}
	render() {
		const { data, loading, selsectValue, advancedValue } = this.state;
		return (
			<div>
				<Tabs defaultActiveKey="2">
					<TabPane tab="模糊搜索" key="1">
						<Search
							style={{ width: '500px' }}
							placeholder="请输入关键字"
							enterButton
							onSearch={(value) => {
								this.setState({ loading: true, data: [] });
								call({
									method: new api.LuceneNetApi().showFields,
									payload: { field: [ 'Name', 'Author', 'Content' ], keyword: value }
								}).then(({ result }) => {
									this.setState({ data: result.items, loading: false });
								});
							}}
						/>
					</TabPane>
					<TabPane tab="按字段搜索" key="2">
						<Select
							style={{ width: 300 }}
							mode="multiple"
							value={selsectValue}
							onChange={(value) => this.setState({ selsectValue: value })}
						>
							<Option value="Name">名称</Option>
							<Option value="Author">作者</Option>
							<Option value="Content">内容</Option>
						</Select>
						<Search
							style={{ width: '500px' }}
							placeholder="请输入关键字"
							enterButton
							onSearch={(value) => {
								this.setState({ loading: true, data: [] });
								call({
									method: new api.LuceneNetApi().showFields,
									payload: { field: selsectValue, keyword: value }
								}).then(({ result }) => {
									this.setState({ data: result.items, loading: false });
								});
							}}
						/>
					</TabPane>
					<TabPane tab="高级搜索" key="3">
						<Form layout="inline">
							{advancedValue.map((x, i) => (
								<Form.Item label={x.name} key={i}>
									<Input
										style={{ width: '500px' }}
										placeholder="请输入关键字"
										value={x.keyword}
										onChange={(e) => {
											let newAdvancedValue = [ ...advancedValue ];
											newAdvancedValue[i].keyword = e.target.value;
											this.setState({ advancedValue: newAdvancedValue });
										}}
									/>
									<Select
										style={{ width: 100 }}
										value={x.occur}
										onChange={(value) => {
											let newAdvancedValue = [ ...advancedValue ];
											newAdvancedValue[i].occur = value;
											this.setState({ advancedValue: newAdvancedValue });
										}}
									>
										<Option value="MUST">与</Option>
										<Option value="SHOULD">或</Option>
										<Option value="MUST_NOT">非</Option>
									</Select>
								</Form.Item>
							))}

							<Form.Item>
								<Button
									type="primary"
									onClick={() => {
										this.setState({ loading: true, data: [] });
										call({
											method: new api.LuceneNetApi().showAdvanced,
											payload: advancedValue
										}).then(({ result }) => {
											this.setState({ data: result.items, loading: false });
										});
									}}
								>
									查询
								</Button>
							</Form.Item>
						</Form>
					</TabPane>
				</Tabs>
				<Table
					style={{ marginTop: 16 }}
					loading={loading}
					dataSource={data}
					rowKey="id"
					columns={[
						{
							title: '名称',
							dataIndex: 'name',
                            key: 'name',
                            width:200,
							render: (t, r) => <div dangerouslySetInnerHTML={{ __html: t }} />
						},
						{
							title: '作者',
							dataIndex: 'author',
							key: 'author',
                            width:150,
							render: (t, r) => <div dangerouslySetInnerHTML={{ __html: t }} />
						},
						{
							title: '内容',
							dataIndex: 'content',
							key: 'content',
							render: (t, r) => <div dangerouslySetInnerHTML={{ __html: t }} />
						}
					]}
				/>
			</div>
		);
	}
}
