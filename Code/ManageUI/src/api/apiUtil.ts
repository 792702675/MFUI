import * as api from './../api/api';
import { notification } from 'antd';
import moment from 'moment';

export interface Parm {
	method: Function;
	payload?: Object;
}

function getArgs(func) {
	//匹配函数括号里的参数
	let args = '';
	if (/function\s.*?\(([^)]*)\)/.test(func.toString())) {
		args = func.toString().match(/function\s.*?\(([^)]*)\)/)[1];
	} else {
		args = func.toString().match(/\(([^)]*)\)/)[1];
	}

	//分解参数成数组
	return args
		.split(',')
		.map(function(arg) {
			//去空格和内联注释
			return arg.replace(/\/\*.*\*\//, '').trim();
		})
		.filter(function(args) {
			//确保没有undefineds
			return args;
		});
}

function eachObject(obj) {
	for (let o in obj) {
		if (moment.isMoment(obj[o])) {
			obj[o] = obj[o].format('YYYY-MM-DD HH:mm:ss');
		} else if (typeof obj[o] == 'object') {
			eachObject(obj[o]);
		}
	}
}

String.prototype.toISOString = function() {
	return this;
};

function callApi(method, params, options): any {
	var arr = getArgs(method).map((x, i, a) => {
		if ((x == 'input' || x == 'model') && i == 0) {
			return params;
		}
		if (x == 'options' && i == a.length - 1) {
			return options;
		}
		return params[x];
	});
	var result = method.apply(this, arr);
	return result
		.then((response) => {
			if (response.json) {
				return response.json();
			}
			return response;
		})
		.catch(function(error) {
			//所有接口的异常除了200-300之间的状态码
			// console.log(error);
			if (error.status === 500) {
				error.text().then((text) => {
					// console.log(text);
					let data = {};
					try {
						data = JSON.parse(text);
					} catch (e) {
						console.log(e);
						notification.error({
							message: '系统异常',
							description: '接口返回数据不是json对象！'
						});
					}
					notification.error({
						message: data.error.message,
						description:
							data.error.details && data.error.details.length > 200
								? data.error.details.substring(0, 200)
								: data.error.details
					});
				});
				// 500状态给返回
				return error;
			}
			// 除了500的抛出给index处理
			throw error;
		});
}

export function createApiAuthParam(parm: Parm): Array<any> {
	let token = localStorage.getItem('token');
	let payload = parm.payload ? parm.payload : {};
	eachObject(payload);

	let headers = {};
	if (token) {
		headers.Authorization = `Bearer ${token}`;
	}
	let options = {
		withCredentials: true,
		credentials: 'include',
		headers: headers
	};
	return [ [ api, callApi ], parm.method, payload, options ];
}

export function call(parm: Parm): any {
	let token = localStorage.getItem('token');
	let payload = parm.payload ? parm.payload : {};
	eachObject(payload);

	let headers = {};
	if (token) {
		headers.Authorization = `Bearer ${token}`;
	}
	let options = {
		withCredentials: true,
		credentials: 'include',
		headers: headers
	};
	return callApi.call(api, parm.method, payload, options);
}
