import { routerRedux } from 'dva/router';
import * as api from './../api/api';
import { notification } from 'antd';
import { createApiAuthParam } from './../api/apiUtil';
import { remoteUrl } from '../utils/url';
export default {
    namespace: 'backknow',
    state: {
        loading: false,
        abled: false,
        hedhtml: '获取验证码'
    },
    reducers: {
        setState(state, { payload }) {
            return {
                ...state,
                ...payload
            };
        },
    },
    effects: {
        *sentpass({ payload }, { call, put, select }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.RestPasswordApi().sendEmailCode,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: '发送成功',
                    description: '验证码发送成功',
                });

            }
        },
        *sentpass2({ payload }, { call, put, select }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.RestPasswordApi().resetPasswordByEmail,
                payload: payload
            }));
            if (success) {
                yield put(routerRedux.push("/savepass/backsucess"));
            }
        },
    },
    subscriptions: {
        setup({ dispatch, history }) {
            return history.listen(({ pathname, state }) => {

            });
        },
    },
};
