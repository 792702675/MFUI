import React from 'react';
import { Form, Input, InputNumber, DatePicker } from 'antd';
const create = Form.create;
const FormItem = Form.Item;
import * as api from '@/api/api';
import PageList from '@/components/PageList/PageList';
import moment from 'moment';

class Demo extends React.Component {
	constructor() {
		super();
	}

	render() {
		const formCol = {
			labelCol: { span: 8 },
			wrapperCol: { span: 12 }
		};

		return (
			<PageList
				ref={(pg) => (this.pageList = pg)}
				form={this.props.form}
				api={new api.DemoApi().getAll}
				deleteApi={new api.DemoApi()._delete}
				updateApi={new api.DemoApi().update}
				createApi={new api.DemoApi().create}
				createPermission="Pages.DemoMangeCreate"
				updatePermission="Pages.DemoMangeUpdate"
				deletePermission="Pages.DemoMangeDelete"
				renderFormNode={(record, getFieldDecorator) => (
					<Form>
						<FormItem label="名称" {...formCol}>
							{getFieldDecorator('name', {
								initialValue: record.name,
								rules: [ { required: true, message: '请填写名称' } ]
							})(<Input />)}
						</FormItem>
						<FormItem label="长文本" {...formCol}>
							{getFieldDecorator('longText', {
								initialValue: record.longText
							})(<Input.TextArea />)}
						</FormItem>
						<FormItem label="排序" {...formCol}>
							{getFieldDecorator('sort', {
								initialValue: record.sort
							})(<InputNumber />)}
						</FormItem>
						<FormItem label="发布时间" {...formCol}>
							{getFieldDecorator('publishTime', {
								initialValue: moment(record.publishTime)
							})(<DatePicker />)}
						</FormItem>
					</Form>
				)}
				filterProps={{
					filters: [
						{
							name: 'name',
							displayName: '名称',
							option: 'like'
						}
					],
					searchProvide: 'sql'
				}}
				listRenderItemType="card"
				renderItem={(item) => <div>{item.name}</div>}
			/>
		);
	}
}
export default create()(Demo);
