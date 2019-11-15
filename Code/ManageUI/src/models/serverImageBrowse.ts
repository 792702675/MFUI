import * as api from './../api/api';
// import * as service from '../services/service';

//requst代码在这里，谁用谁接
// export async function getServerImage(options) {
//     return request(
//         `/UE/controller.ashx?action=listimage&start=${options.param.start}&size=${options.param
//             .size}&noCache=${Math.random()}`,
//         options
//     );
// }

export default {
    namespace: 'serverImageBrowse',
    state: {
        items: [],
        total: 0,
        start: 0,
        size: 1000000//16
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
        *getAll({ payload }, { call, put, select }) {
            const state = yield select(({ serverImageBrowse }) => serverImageBrowse);
            const result = yield call(service.getServerImage, {
                method: 'get',
                param: { start: payload.start, size: state.size }
            });
            if (result.state == 'SUCCESS') {
                yield put({
                    type: 'setState',
                    payload: {
                        items: payload.start == 0 ? result.list : [...state.items, ...result.list],
                        total: result.total,
                        start: state.start + state.size
                    }
                });
            }
        }
    }
};
