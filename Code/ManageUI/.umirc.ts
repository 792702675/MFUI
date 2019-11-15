import { IConfig } from 'umi-types';

// ref: https://umijs.org/config/
const config: IConfig = {
	treeShaking: true,
	// uglifyJSOptions: { compress: false },
	uglifyJSOptions(opts) {
		opts.uglifyOptions.mangle = false;
		return opts;
	},
	plugins: [
		// ref: https://umijs.org/plugin/umi-plugin-react.html
		[
			'umi-plugin-react',
			{
				antd: true,
				dva: true,
				dynamicImport: false,
				title: 'UmiMfui',
				dll: false,

				routes: {
					exclude: [ /models\//, /services\//, /model\.(t|j)sx?$/, /service\.(t|j)sx?$/, /components\// ]
				}
			}
		]
	],
	publicPath: '/',
	// base: '/admin',
	history: 'hash',
	// exportStatic: {
	//   // htmlSuffix: true,
	//   dynamicRoot: true,
	// },
	proxy: {
		'/api': {
			target: 'http://220.165.143.88:5601/',
			changeOrigin: true,
			ws: true,
			pathRewrite: { '^/api': '' }
		},
		'/UE': {
			target: 'http://220.165.143.88:5601/',
			changeOrigin: true,
			pathRewrite: { '^/UE': '/UE' }
		},
		'/Common': {
			target: 'http://220.165.143.88:5601/',
			changeOrigin: true,
			pathRewrite: { '^/Common': '/Common' }
		}
	}
};

export default config;
