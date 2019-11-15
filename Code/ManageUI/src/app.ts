import { createLogger } from 'redux-logger';
import { notification } from 'antd';
import { routerRedux } from 'dva/router';

export const dva = {
	config: {
		// onAction: createLogger(),
		onError(e) {
			if (e.status == 400) {
				var response = e.json();
				response.then(function(data) {
					var detail = data.error.details;
					if (data.error.validationErrors && data.error.validationErrors.length) {
						detail = '';
						for (var i in data.error.validationErrors) {
							detail += data.error.validationErrors[i].message;
						}
					}
					notification.error({
						message: data.error.message,
						description: detail && detail > 200 ? detail.substring(0, 200) : detail
					});
				});
			} else if (e.status == 401) {
				notification.error({
					message: '用户未登录',
					description: '当前用户没有登录到系统！',
					key: '401_error_notification'
				});
				window.g_app._store.dispatch(routerRedux.push('/'));
			} else if (e.status == 403) {
				notification.error({
					message: '用户权限不足',
					description: '当前用户没有此项操作的权限！',
					key: '403_error_notification'
				});
				window.g_app._store.dispatch(routerRedux.push('/'));
			} else if (e.status == 500) {
				var response = e.json();
				response.then(function(data) {
					notification.error({
						message: data.error.message,
						description:
							data.error.details && data.error.details.length > 200
								? data.error.details.substring(0, 200)
								: data.error.details
					});
				});
			}
		}
	}
};
