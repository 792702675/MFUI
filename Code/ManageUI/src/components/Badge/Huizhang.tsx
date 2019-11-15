import React from 'react';
import { connect } from 'dva';
import { Form, Card, Button, Icon, Select } from 'antd';
const create = Form.create;
const FormItem = Form.Item;

import * as api from '../../api/api';
import EnumSelect from '@/components/System/EnumSelect';

export default class Huizhang extends React.Component {
    render() {
        const { value, onChange } = this.props;
        return (
            <div>
                    <EnumSelect
                    value={value}
                        onChange={(e) => {
                            let newValue = e;
                            onChange(newValue);
                        }}
                        api={new api.CollectionApi().getDropDownList}
                        payload={{ type: "徽章" }}
                        optionRender={(x) => (
                            <Select.Option value={x.value} key={x.iconUrl}>
                                <img src={x.iconUrl} style={{ height: '25px' }} /> {x.name}
                            </Select.Option>
                        )}
                    />
            </div>
        );
    }
}
