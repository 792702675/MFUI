import React from 'react';
// import * as service from '../services/service';
import { routerRedux } from 'dva/router';
import { remoteUrl, homePageUrl } from '../utils/url';
import { notification } from 'antd';
import * as api from './../api/api';
import { createApiAuthParam } from './../api/apiUtil';

export default {
	namespace: 'indexpage',
	state: {
		setting: {}, //所有配置项
		thirdPartyList: [],
		thirdPartyToken: '',
		goToCode: 0 // 需要激活跳转编码， 1 表示需要激活手机号， 2 表示需要激活邮箱，  3 表示需要激活（1+2）
	},

	effects: {
		*login({ payload }, { call, put }) {
			// const data = yield call(service.login2, {
			// 	method: 'post',
			// 	body: payload.values
			// });
			const { success, result, error } = yield call(
				...createApiAuthParam({
					method: new api.TokenAuthApi().authenticate,
					payload: payload.values
				})
			);
			if (success) {
				// alert()
				localStorage.setItem('token', result.accessToken);
				yield put({
					type: 'home/setState',
					payload: {
						shouldChangePasswordOnNextLogin: result.shouldChangePasswordOnNextLogin
					}
				});
					yield put(routerRedux.push(homePageUrl));
			} else {
				payload.callback();
				//  检查是否要跳转到激活页面。
				var codesOfNeedGo = [ 1, 2, 3 ];
				if (error && error.code && codesOfNeedGo.indexOf(error.code) != -1) {
					yield put(
						routerRedux.push({
							pathname: `/active`,
							state: {
								goToCode: error.code
							}
						})
					);
				}
			}
		},
		*thirdPartyList({ payload }, { call, put }) {
			const { success, result } = yield call(service.thirdPartyList, {
				method: 'post'
			});
			if (success) {
				yield put({
					type: 'setState',
					payload: { thirdPartyList: result }
				});
			}
		},
		*thirdPartyLogin({ payload }, { call, put }) {
			const { success, result } = yield call(service.thirdPartyLogin, {
				method: 'post',
				body: payload
			});
			if (success) {
				if (result.success) {
					location.href = `${location.protocol}//${location.host}/index.html#${homePageUrl}`;
				} else if (result.requireCreateNewUser) {
					localStorage.thirdPartyToken = result.token;
					location.href = `${location.protocol}//${location.host}/index.html#/thirdpartybinding`;
				} else {
					notification.error({
						message: '认证失败!',
						description: '认证失败'
					});
				}
			}
		},
		*GetClientSetting({}, { call, put }) {
			const { success, result } = yield call(
				...createApiAuthParam({
					method: new api.ConfigurationApi().getClientSetting,
					payload: {}
				})
			);
			if (success) {
				yield put({
					type: 'setState',
					payload: { setting: result }
				});
			}
		}
	},
	subscriptions: {
		setup({ dispatch, history }) {
			return history.listen(({ pathname, state }) => {
				if (pathname.toLowerCase() == '/'.toLowerCase()) {
					// alert()
					dispatch({
						type: 'GetClientSetting'
					});
				}
			});
		}
	},
	reducers: {
		save(state, action) {
			return { ...state, ...action.payload };
		},
		setState(state, { payload }) {
			return {
				...state,
				...payload
			};
		}
	}
};
