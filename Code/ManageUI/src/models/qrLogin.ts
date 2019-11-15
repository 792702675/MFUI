import { routerRedux } from 'dva/router';
import { remoteUrl, homePageUrl } from '../utils/url';
import * as signalR from '@aspnet/signalr';
import * as api from './../api/api';
import { createApiAuthParam } from './../api/apiUtil';

export default {
	namespace: 'qrLogin',
	state: {
		url: null,
		scanQRCode: false
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
		*init({ payload }, { call, put }) {
			const connection = new signalR.HubConnectionBuilder()
				.withUrl(remoteUrl + '/qrLoginHub', { transport: signalR.HttpTransportType.LongPolling })
				.build();

			connection
				.start()
				.then(function() {
					window.g_app._store.dispatch({
						type: 'qrLogin/showQRCode'
					});
					console.log('Connected to SignalR server!'); //TODO: Remove log
				})
				.catch((err) => {
					console.log(err);
				});
			connection.on('scanQRCode', function() {
				window.g_app._store.dispatch({
					type: 'qrLogin/setState',
					payload: {
						scanQRCode: true
					}
				});
			});
			connection.on('confirmLogin', function(token) {
				window.g_app._store.dispatch({
					type: 'qrLogin/qrLogin',
					payload: { token: token }
				});
			});
			window.connection = connection;
		},
		*showQRCode({ payload }, { call, put }) {
			window.connection.invoke('getToken').then(function(result) {
				if (result) {
					console.log(result);
					window.g_app._store.dispatch({
						type: 'qrLogin/setState',
						payload: {
							url: `${remoteUrl}/qrLogin.html?connectionId=${result.connectionId}&token=${result.token}`
						}
					});
				}
			});
			setTimeout(() => {
				window.g_app._store.dispatch({
					type: 'qrLogin/showQRCode'
				});
			}, 180000);
		},
		*qrLogin({ payload }, { call, put }) {
			const { success, result } = yield call(
				...createApiAuthParam({
					method: new api.QRLoginApi().login,
					payload: payload
				})
			);
			if (success) {
				localStorage.setItem('token', result.accessToken);
				yield put({
					type: 'setState',
					payload: {
						scanQRCode: false
					}
				});
				window.connection.stop();
				yield put(routerRedux.push(homePageUrl));
			}
		}
	},
	subscriptions: {
		setup({ dispatch, history }) {
			return history.listen(({ pathname, state }) => {
				if (pathname.toLowerCase() == '/activation/qrLogin'.toLowerCase()) {
					dispatch({
						type: 'init'
					});
				}
			});
		}
	}
};
