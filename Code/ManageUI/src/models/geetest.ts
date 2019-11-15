import * as api from './../api/api';
import { createApiAuthParam } from './../api/apiUtil';
import React from 'react';

export default {
    namespace: 'geetest',
    state: {},
    reducers: {
        setState(state, { payload }) {
            return {
                ...state,
                ...payload
            };
        }
    },
    effects: {
        *getCaptcha({ payload }, { call, put }) {
            const { success, result } = yield call(
                ...createApiAuthParam({
                    method: new api.GeetestApi().getCaptcha
                })
            );
            if (success) {
                payload.callback(JSON.parse(result));
            }
        },
        *check({ payload }, { call, put, select }) {
            const { success, result } = yield call(
                ...createApiAuthParam({
                    method: new api.GeetestApi().check,
                    payload: {
                        ...payload
                    }
                })
            );
            if (success) {
                if (result.success) {
                    payload.callback(result.token);
                }
            }
        },
        *reset({ payload }, { call, put, select }) {
            window.captchaObj.reset();
            window.captchaObj.props.onChange('');
        }
    }
};
