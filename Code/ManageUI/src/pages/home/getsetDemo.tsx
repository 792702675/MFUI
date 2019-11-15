import React, { Component, PropTypes } from 'react';
import { connect } from 'dva';
import { Form, Input, InputNumber } from 'antd';

const create = Form.create;
const FormItem = Form.Item;
import * as api from '../../api/api';

import GetSet from '../../components/GetSet/GetSet';
import moment from 'moment';

function GetsetDemo({ form, data }) {
	const { getFieldDecorator } = form;
	const formCol = {
		labelCol: { span: 8 },
		wrapperCol: { span: 12 }
	};
	const formNode = (
		<Form>
			<FormItem label="文件大小" {...formCol}>
				{getFieldDecorator('fileSize', {
					initialValue: data.fileSize
				})(<InputNumber />)}
			</FormItem>
		</Form>
	);

	return (
		<GetSet
			form={form}
			getApi={new api.FileSettingDemoApi().get1}
			setApi={new api.FileSettingDemoApi().set}
			formNode={formNode}
		/>
	);
}

GetsetDemo = connect((state) => {
	return {
		...state.getSet
	};
})(GetsetDemo);
export default create()(GetsetDemo);
