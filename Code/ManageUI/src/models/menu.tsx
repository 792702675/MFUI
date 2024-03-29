import React from 'react';
import { notification } from 'antd';

import * as api from './../api/api';
import { createApiAuthParam } from './../api/apiUtil';

export default {
	namespace: 'menu',
	state: {
		expand: false,
		editMenuModalVisible: false,
		menus: [], //菜单列表
		menu: {},
		parentId: 0,
		permissions: [],
		isEditingMenu: false
	},
	reducers: {
		setState(state, { payload }) {
			return {
				...state,
				...payload
			};
		}
	},
	effects: {
		*createCustomMenu({ payload }, { call, put }) {
			const { success, result } = yield call(
				...createApiAuthParam({
					method: new api.MenuApi().createCustomMenu,
					payload: payload
				})
			);
			if (success) {
				notification.success({
					message: '添加成功!',
					description: (
						<span>
							您已成功添加菜单 <strong> {payload.displayName} </strong>,所做的修改将会在刷新后生效
						</span>
					)
				});
				yield put({
					type: 'setState',
					payload: {
						editMenuModalVisible: false
					}
				});
				yield put({
					type: 'getMenus'
				});
			}
		},
		*updateMenu({ payload }, { call, put }) {
			const { success, result } = yield call(
				...createApiAuthParam({
					method: new api.MenuApi().updateMenu,
					payload: payload
				})
			);
			if (success) {
				notification.success({
					message: '修改成功!',
					description: (
						<span>
							您已成功修改菜单 <strong> {payload.displayName} </strong>,所做的修改将会在刷新后生效
						</span>
					)
				});
				yield put({
					type: 'setState',
					payload: {
						editMenuModalVisible: false,
						isEditingMenu: false
					}
				});
				yield put({
					type: 'getMenus'
				});
			}
		},
		*moveMenu({ payload }, { call, put }) {
			const { success, result } = yield call(
				...createApiAuthParam({
					method: new api.MenuApi().moveMenu,
					payload: payload
				})
			);
			if (success) {
				notification.success({
					message: '移动成功!',
					description: <span>您已成功移动菜单, 所做的修改将会在刷新后生效</span>
				});
				yield put({
					type: 'getMenus'
				});
			}
		},
		*deleteMenu({ payload }, { call, put }) {
			const { success, result } = yield call(
				...createApiAuthParam({
					method: new api.MenuApi().deleteMenu,
					payload: payload
				})
			);
			if (success) {
				notification.success({
					message: '删除成功!',
					description: <span>您已成功删除菜单, 所做的修改将会在刷新后生效</span>
				});
				yield put({
					type: 'getMenus'
				});
			}
		},
		*getMenus({ payload }, { call, put }) {
			const { success, result } = yield call(
				...createApiAuthParam({
					method: new api.MenuApi().getAllMenus,
					payload: payload
				})
			);
			if (success) {
				// console.log(result)
				yield put({
					type: 'setState',
					payload: {
						menus: result
					}
				});
			}
		},
		*getAllPermissions({ payload }, { call, put }) {
			const { success, result } = yield call(
				...createApiAuthParam({
					method: new api.PermissionApi().getAllPermissionTree,
					payload: payload
				})
			);
			if (success) {
				yield put({
					type: 'setState',
					payload: {
						permissions: result
					}
				});
			}
		}
	},
	subscriptions: {
		setup({ dispatch, history }) {
			return history.listen(({ pathname, state }) => {
				if (pathname.toLowerCase() == '/home/menu'.toLowerCase()) {
					dispatch({
						type: 'getMenus'
					});
					dispatch({
						type: 'getAllPermissions'
					});
				}
			});
		}
	}
};
