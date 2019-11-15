import React, { Component, PropTypes } from 'react';
import { connect } from 'dva';
import { Link } from 'dva/router';
import { Form, Icon, Button } from 'antd';
import styles from '../../style/activation.css';
function Callsucess() {

    return (
        <div>
            <Form className={`${styles.formbox} login-form`}>
                <div style={{ marginTop: 20, height: '80px', lineHeight: '80px' }}><span
                    style={{ fontSize: '50px', color: 'red', float: 'left', fontWeight: '900' }}><Icon
                        type="check-circle-o" /></span><span
                            style={{ fontSize: '30px', color: '#000', marginLeft: 20, float: 'left', fontWeight: '900' }}>成功</span></div>
                <Link to="/"><Button type="primary" htmlType="submit" className={styles.login}>
                    前往登录
        </Button></Link>
            </Form>
        </div>
    )
}

export default Callsucess;
