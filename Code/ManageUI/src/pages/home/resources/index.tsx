import * as React from 'react';
import { connect } from 'dva';
import { Card, Icon, Avatar, Checkbox, Row, Col, Button, Modal, Form, Input, Switch, Radio, Select, message } from 'antd';
const create = Form.create;
const { Meta } = Card;
import { Link } from 'dva/router';
import TagSelect from '@/components/System/TagSelect';
const confirm = Modal.confirm;
const { TextArea } = Input;
const { Option } = Select;
function Oss({ dispatch, buckets, visible, form, bucketName, can, buckitem, deletebucketvisible }) {
    const { getFieldDecorator } = form;
    const formCol = {
        labelCol: { span: 6 },
        wrapperCol: { span: 16 }
    };
    let handleCancel = () => {
        dispatch({
            type: 'oss/setState',
            payload: {
                visible: false
            }
        })
    }
    let handleSubmit = e => {
        e.preventDefault();
        form.validateFields((err, values) => {
            if (!err) {
                dispatch({
                    type: 'oss/CreateBucket',
                    payload: {
                        ...values
                    }
                })
                // console.log('Received values of form: ', values);
            }
        });
    }
    const optionsWithDisabled = [
        { label: 'get', value: 'get' },
        { label: 'post', value: 'post' },
        { label: 'put', value: 'put' },
        { label: 'delete', value: 'delete' },
        { label: 'head', value: 'head' },
    ]
    return (
        <div style={{ height: '100%' }}>
            {
                can ? <Button type="primary" icon="plus" onClick={() => dispatch({
                    type: "oss/setState",
                    payload: {
                        visible: true
                    }
                })}>新增存储库</Button> : null
            }

            <Row gutter={16} style={{ marginTop: '20px' }}>
                {
                    buckets ? buckets.map((items, index) => (
                        <Col xl={6} lg={12} style={{ marginBottom: '20px' }} key={items.name + "www"}>
                            <Card
                                actions={[<Icon type="delete" onClick={() => dispatch({
                                    type: "oss/deletebucketvisible",
                                    payload: {
                                        deletebucketvisible: true,
                                        buckitem: items
                                    }
                                })} />]}
                                key={items.name}
                            >
                                <Meta
                                    avatar={<Icon type="menu" style={{ fontSize: '26px' }} />}
                                    title={<Link to="/home/resources/file" style={{ cursor: 'pointer' }} onClick={() => {
                                        window.sessionStorage.setItem('bucketName', items.name)
                                        window.sessionStorage.setItem('root', '')
                                    }}>{items.name}</Link>}
                                    description={items.location}
                                />
                            </Card>
                        </Col>
                    )) : null
                }
            </Row>
            <Modal
                title={"删除确认"}
                visible={deletebucketvisible}
                onOk={() => {
                    if (buckitem.name == form.getFieldValue("deletefilename")) {
                        dispatch({
                            type: "oss/DeleteBucket",
                            payload: {
                                bucketName: buckitem.name
                            }
                        })
                    } else {
                        message.info('输入名称和原存储库名不符，删除失败！')
                    }

                }}
                onCancel={() => {
                    dispatch({
                        type: "oss/setState",
                        payload: {
                            deletebucketvisible: false
                        }
                    })
                    form.resetFields();
                }}
            >
                {getFieldDecorator('deletefilename', {

                })(
                    <Input placeholder="请输入您需要的存储库名称" />
                )
                }
            </Modal>
            <Modal
                title="新增存储库"
                visible={visible}
                onOk={handleSubmit}
                onCancel={handleCancel}
                width={700}
                footer={[
                    <Button key="back" onClick={handleCancel}>
                        取消
                    </Button>,
                    <Button key="submit" type="primary" onClick={handleSubmit}>
                        确定
                    </Button>
                ]}
            >
                <Form onSubmit={handleSubmit}>
                    <Form.Item label="名称" {...formCol}>
                        {getFieldDecorator('bucketName', {

                            rules: [{
                                pattern: /^[a-z0-9][a-z0-9-]{1,61}[a-z0-9]$/,
                                message: '格式不符合要求'
                            }],
                        })(
                            <Input
                                placeholder="请填写英文名称，字符不超过30个"
                            />,
                        )}
                        <div>只能包含小写字母、数字或短划线（ - ）; 以小写字母或数字开头和结尾;长度必须介于3到63之间</div>
                    </Form.Item>
                    <Form.Item label="说明" {...formCol}>
                        {getFieldDecorator('note', {
                        })(
                            <TextArea rows={4} />
                        )}
                    </Form.Item>
                </Form>
            </Modal>
        </div>
    );
}

Oss = connect((state) => {
    return {
        ...state.oss
    };
})(Oss);
export default create()(Oss);
