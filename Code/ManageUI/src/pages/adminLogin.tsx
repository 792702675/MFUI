import React from 'react';
import styles from './index.css';
import Link from 'umi/link';
import { Form, Icon, Input, Button, Checkbox, notification, Row, Col, Tooltip } from 'antd';
const FormItem = Form.Item;
import { connect } from 'dva';
import Geetest from './../components/Geetest/Geetest';
const create = Form.create;
function Index({ dispatch, form, setting, thirdPartyList, loading }) {
  const { getFieldDecorator } = form;
  let handleSubmit = (e) => {
    e.preventDefault();
    form.validateFields((err, values) => {
      if (!err) {
        // console.log('Received values of form: ', values);
        dispatch({
          type: 'indexpage/login',
          payload: { 
            values: values, 
            // callback: this.refreshCaptcha 
          }
        });
      }
    });
  };
  return (
    <div className={styles.navbox}>
      <div className={styles.logologin}>{setting.systemName ||'后台管理系统'}</div>
      <div className={styles.indexbox}>
        {/* <div style={{ position: 'absolute', right: '24px' }}>
          <Tooltip placement="left" title="二维码登录">
            <Link to="/activation/qrlogin">
              <Icon type="qrcode" style={{ fontSize: 30, color: '#08c' }} />
            </Link>
          </Tooltip>
        </div> */}
        <header className={styles.headerbox}>请登录</header>
        <Form onSubmit={handleSubmit} className={`${styles.formbox} login-form`}>
          <FormItem>
            <div className={styles.colorsize}>用户名：</div>
            {getFieldDecorator('usernameOrEmailAddress', {
              rules: [{ required: true, message: '请输入用户名！' }]
            })(<Input prefix={<Icon type="user" style={{ fontSize: 13 }} />} placeholder="用户名" style={{ marginTop: '5px', height: 40 }} />)}
          </FormItem>
          <FormItem>
            <span className={styles.colorsize}>登录密码：</span>
            {/* <Link to="savepass/backknow" className={styles.forgetpassword} href="">
              忘记密码？
							</Link> */}
            {getFieldDecorator('password', {
              rules: [{ required: true, message: '请输入密码！' }]
            })(<Input prefix={<Icon type="lock" style={{ fontSize: 13 }} />} type="password" placeholder="密码" style={{ marginTop: '5px', height: 40 }} />)}
          </FormItem>


          <FormItem>
            <span className={styles.colorsize}>验证码：</span>
            <center>
              {getFieldDecorator('captcha', {
                rules: [{ required: true, message: '请点击验证！' }]
              })(<Geetest />)}
            </center>
          </FormItem>
          <FormItem>
            {getFieldDecorator('rememberMe', {
              valuePropName: 'checked',
              initialValue: true
            })(<Checkbox>记住密码</Checkbox>)}

            <Button type="primary" loading={loading} htmlType="submit" className={styles.login}>
              登录
							</Button>
          </FormItem> 
        </Form>
        {/* <div className={styles.reB}>
          {setting && setting.allowSelfRegistration ? (<div className={styles.reBL}>
            <span>还没有账号？</span>
            <Link to="/register" className={styles.newuser}>
              马上注册
								</Link>
          </div>) : (<div className={styles.reBL} />)}
          <Link to="/activation/sendemail" style={{ float: 'right' }}>
            激活
						</Link>
        </div> */}
      </div>
    </div>
  );
}

Index = connect((state) => {
  return {
    ...state.indexpage
  };
})(Index);

export default create()(Index);
