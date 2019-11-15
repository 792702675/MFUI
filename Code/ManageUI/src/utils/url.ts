export const remoteUrl =
	location.host.indexOf('localhost:800') >= 0
		? `${location.protocol}//${location.host}/api`
		: `${location.protocol}//${location.host}`;
export const homePageUrl = '/home';
