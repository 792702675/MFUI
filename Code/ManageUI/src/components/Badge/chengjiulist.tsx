import React from 'react';
import { connect } from 'dva';
import { Button, Card, Row, Col, Input, Form, List, Avatar } from 'antd';
import { remoteUrl } from '../../utils/url';
import * as api from '../../api/api';
import { call } from '../../api/apiUtil';
import styles from '../../../style/microworld.css';
import CollectionItem from '@/components/Home/CollectionItem';
import CollectionItemInput from './achievements';
import Huizhang from './Huizhang';
import { relative } from 'path';
const create = Form.create;
const FormItem = Form.Item;
class HumanBodys extends React.Component {
    constructor() {
        super();
        this.state = {
            cardlist: [],
            index: '',
            bagetype:[]
        };
    }

    componentDidMount() {
        this.setState({
            cardlist: this.props.value ? this.props.value : []
        })
        call({
            method: new api.CollectionApi().getDropDownList,
            payload: {
                type: "徽章"
            }
        }).then(({ result }) => {
            this.setState({
                bagetype:result
            })
        });
        // this.props.onChange(this.state.cardlist)
    }

    handleSubmit = e => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            let arr = this.state.cardlist;
            if (!err) {
                if (this.state.index !== '') {
                    arr.splice(this.state.index, 1)
                }
                for (var i = 0; i < this.state.bagetype.length;i++){
                    if (values.collectionId == this.state.bagetype[i].value){
                        values.collection={
                            name: this.state.bagetype[i].name,
                            ossETagIcon:{
                                url: this.state.bagetype[i].iconUrl
                            }
                        }
                    }
                }
                arr.push(values)
                this.setState({
                    cardlist: [...arr],
                    index: ''
                })
                this.props.form.resetFields()
            }
        });
        this.props.onChange(this.state.cardlist)
    };

    render() {

        const { cardlist } = this.state;
        const { getFieldDecorator } = this.props.form;
        const formCol = {
            labelCol: { span: 3 },
            wrapperCol: { span: 16 }
        };
        return (
            <div>
                {cardlist ? cardlist.map((x, i) => (
                    <div key={i}>
                        <Row>
                            <Col offset={3} span={16}>
                                <Card
                                    style={{ margin: '8px' }}
                                    size="small"
                                    title={<div><img src={x.collection.ossETagIcon.url} style={{width:38,height:38,marginRight:10}}/><span>{x.collection.name}</span></div>}
                                    extra={[<a onClick={() => {
                                        this.setState({ index: i })
                                        this.props.form.setFieldsValue({ 'collectionId': x.collectionId, badgeConditions: x.badgeConditions })
                                    }} style={{ marginRight: 10 }}>编辑</a>, <a onClick={() => {
                                        let arr = this.state.cardlist;
                                        arr.splice(i, 1)
                                        this.setState({
                                            cardlist: [...arr]
                                        })
                                        this.props.onChange(arr)
                                    }}>移除</a>]}
                                >
                                    {x.badgeConditions ? x.badgeConditions.map((item, index) => (
                                        <div>
                                            {item.conditionNote ? item.conditionNote + ":" + item.condition : item.conditionType + ":" + item.condition}
                                        </div>
                                    )) : null}
                                </Card>
                            </Col>
                        </Row>
                    </div>
                )) : null}
                <Row>
                    <Form onSubmit={this.handleSubmit}>
                        <FormItem label="徽章" {...formCol}>
                            {getFieldDecorator('collectionId', {
                                // initialValue: record.normalFactor,
                                rules: [{ required: true, message: '请选择徽章' }]
                            })(<Huizhang />)}
                        </FormItem>
                        <FormItem label="条件" {...formCol}>
                            {getFieldDecorator('badgeConditions', {
                                // initialValue: this.props.value ? this.props.value.badgeConditions:null,
                                rules: [{ required: true, message: '请填写达成条件' }]
                            })(<CollectionItemInput api={new api.BadgeApi().getConditionTypeList}/>)}
                        </FormItem>
                    </Form>
                </Row>
                <Col offset={3} span={16}>
                    <Button type="primary" htmlType="submit" onClick={this.handleSubmit}>确定</Button>
                    <Button style={{ marginLeft: 10 }} onClick={() => { this.props.form.resetFields(); this.setState({ index: '' }) }}>放弃</Button>
                </Col>
            </div>
        );
    }
}

export default create()(HumanBodys);
