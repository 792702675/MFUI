import React from 'react';
import { connect } from 'dva';
import { Form, Card, Button, Icon, Select } from 'antd';
const create = Form.create;
const FormItem = Form.Item;
import 'pinyin4js';

import * as api from '../../api/api';
import EnumSelect from '@/components/System/EnumSelect';

export default class CollectionItem extends React.Component {
	render() {
		const { value = [], onChange, type } = this.props;
		return (
			<div>
				{value.map((x, i) => (
					<div key={i}>
						<Card
							style={{ margin: '8px' }}
							size="small"
							title={'藏品' + (i + 1)}
							extra={
								<Icon
									type="close"
									style={{ cursor: 'pointer' }}
									onClick={() => {
										let newValue = [ ...value ];
										newValue.splice(i, 1);
										onChange(newValue);
									}}
								/>
							}
						>
							<EnumSelect
								value={x}
								onChange={(e) => {
									let newValue = [ ...value ];
									newValue[i] = e;
									onChange(newValue);
								}}
								api={new api.CollectionApi().getDropDownList}
								payload={{ type: type }}
								filterOption={(input, option) =>
									option.props.name.toLowerCase().indexOf(input.toLowerCase()) >= 0 ||
									PinyinHelper.convertToPinyinString(
										option.props.name.toLowerCase(),
										'',
										PinyinFormat.FIRST_LETTER
									)
										.toString()
										.replace(/,/g, '')
										.indexOf(input.toLowerCase()) >= 0 ||
									PinyinHelper.convertToPinyinString(
										option.props.name.toLowerCase(),
										'',
										PinyinFormat.WITHOUT_TONE
									)
										.toString()
										.replace(/,/g, '')
										.indexOf(input.toLowerCase()) >= 0}
								optionRender={(x, i) => (
									<Select.Option key={i} value={x.value} name={x.name}>
										<img src={x.iconUrl} style={{ height: '25px' }} /> {x.name}
									</Select.Option>
								)}
							/>
						</Card>
					</div>
				))}
				<center>
					<Button
						onClick={() => {
							let newValue = [ ...value, null ];
							onChange(newValue);
						}}
					>
						添加
					</Button>
				</center>
			</div>
		);
	}
}
