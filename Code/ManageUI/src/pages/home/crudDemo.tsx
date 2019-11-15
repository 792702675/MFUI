import React from 'react';
import { connect } from 'dva';
import { Form, Input, InputNumber, Badge, DatePicker } from 'antd';
const create = Form.create;
const FormItem = Form.Item;

const { TextArea } = Input;
import * as api from '../../api/api';
import moment from 'moment';
import CRUD from "../../components/CRUD/CRUD"
function Demo({ form, record }) {
    const columns = [
        {
            title: '名称',
            dataIndex: 'name',
            sorter: true
        },
        {
            title: '长文本',
            dataIndex: 'longText',
            sorter: true
        },
        {
            title: '启用',
            dataIndex: 'isActivate',
            sorter: true,
            render: (text, record) =>
                text ? <Badge status="default" text="错误" /> : <Badge status="success" text="成功" />
        },
        {
            title: '排序',
            dataIndex: 'sort',
            sorter: true
        },
        {
            title: '经度',
            dataIndex: 'location.longitude',
            sorter: true
        },
        {
            title: '纬度',
            dataIndex: 'location.latitude',
            sorter: true
        },
        {
            title: '发布时间',
            dataIndex: 'publishTime',
            sorter: true,
            render: (text, record) => (text ? <span>{moment(text).format('YYYY-MM-DD HH:mm')}</span> : null)
        },
        {
            title: '添加时间',
            dataIndex: 'creationTime',
            sorter: true,
            render: (text, record) => (text ? <span>{moment(text).format('YYYY-MM-DD HH:mm')}</span> : null)
        },
        {
            title: '最后一次修改时间',
            dataIndex: 'lastModificationTime',
            sorter: true,
            render: (text, record) => (text ? <span>{moment(text).format('YYYY-MM-DD HH:mm')}</span> : null)
        }
    ];
    const { getFieldDecorator } = form;
    const formCol = {
        labelCol: { span: 8 },
        wrapperCol: { span: 12 }
    };
    const formNode = (
        <Form>
            <FormItem label="名称" {...formCol}>
                {getFieldDecorator('name', {
                    initialValue: record.name,
                    rules: [{ required: true, message: '请填写名称' }]
                })(<Input />)}
            </FormItem>
            <FormItem label="长文本" {...formCol}>
                {getFieldDecorator('longText', {
                    initialValue: record.longText
                })(<TextArea />)}
            </FormItem>
            <FormItem label="排序" {...formCol}>
                {getFieldDecorator('sort', {
                    initialValue: record.sort
                })(<InputNumber />)}
            </FormItem>
            <FormItem label="经度" {...formCol}>
                {getFieldDecorator('location.longitude', {
                    initialValue: record.location && record.location.longitude
                })(<InputNumber />)}
            </FormItem>
            <FormItem label="纬度" {...formCol}>
                {getFieldDecorator('location.latitude', {
                    initialValue: record.location && record.location.latitude
                })(<InputNumber />)}
            </FormItem>
            <FormItem label="发布时间" {...formCol}>
                {getFieldDecorator('publishTime', {
                    initialValue: moment(record.publishTime)
                })(<DatePicker />)}
            </FormItem>
        </Form>
    );
    const filters = [
        {
            name: 'name',
            displayName: '名称',
            option: 'like'
        },
        {
            name: 'creationTime',
            displayName: '添加时间',
            type: 'datetime',
            option: '>='
        },
        {
            name: 'creationTime',
            displayName: '',
            type: 'datetime',
            option: '<'
        }
    ];

    return (
        <CRUD
            form={form}
            getAllApi={new api.DemoApi().getAll}
            deleteApi={new api.DemoApi()._delete}
            updateApi={new api.DemoApi().update}
            createApi={new api.DemoApi().create}
            deleteBatchApi={new api.DemoApi().deleteBatch}
            createPermission="Pages.DemoMangeCreate"
            updatePermission="Pages.DemoMangeUpdate"
            deletePermission="Pages.DemoMangeDelete"
            columns={columns}
            formNode={formNode}
            filterProps={{
                filters,
                searchProvide: 'sql'
            }}
        />
    );
}

Demo = connect((state) => {
    return {
        ...state.crud
    };
})(Demo);
export default create()(Demo);