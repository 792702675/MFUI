import React from 'react';
import { remoteUrl } from '../../utils/url';

export default class PDFViewer extends React.Component {
	constructor(props) {
		super(props);
		this.viewerRef = React.createRef();
	}

	componentDidMount() {
		this.loading = true;
		const src = encodeURIComponent(this.props.url && this.props.url.indexOf('//') >= 0 ? this.props.url : remoteUrl + this.props.url);
		const element = this.viewerRef.current;

		const iframe = document.createElement('iframe');
		iframe.src = `/pdfjs/web/viewer.html?file=${src}`;
		iframe.width = '100%';
		iframe.height = '100%';
		iframe.style = 'border:none;';
		element.appendChild(iframe);
		this.iframe = iframe;

		const usedTime = 5;

		const _this = this;

		function onLoad() {
			if (_this.loadingTimer) {
				clearInterval(_this.loadingTimer);
			}
			_this.loading = false;
			if (_this.page != undefined) {
				_this.iframe.contentWindow.PDFViewerApplication.page = _this.page;
			}
			if (_this.props.recordEvent || _this.props.onLoad) {
				_this.timer = setInterval(() => {
					_this.props.recordEvent && _this.props.recordEvent({
						materialId: _this.props.id,
						endLocation: _this.iframe.contentWindow.PDFViewerApplication.page,
						usedTime: usedTime
					});
					_this.props.onLoad && _this.props.onLoad(_this.iframe.contentWindow)
				}, usedTime * 1000);
			}
		}

		_this.loadingTimer = setInterval(() => {
			if (
				_this.iframe &&
				_this.iframe.contentWindow &&
				_this.iframe.contentWindow.PDFViewerApplication &&
				_this.iframe.contentWindow.PDFViewerApplication.page
			) {
				onLoad();
			}
		}, 1000);
	}
	componentWillUpdate(nextProps, nextState) {
		if (nextProps.endLocation != this.props.endLocation) {
			if (!this.loading) {
				this.iframe.contentWindow.PDFViewerApplication.page = nextProps.endLocation;
			} else {
				this.page = nextProps.endLocation;
			}
		}
	}
	componentWillUnmount() {
		if (this.timer) {
			clearInterval(this.timer);
		}
	}
	componentWillReceiveProps(nextProps){
		if(this.props.url!== nextProps.url){
			if (this.timer) {
				clearInterval(this.timer);
			}
			const element = this.viewerRef.current;
			element.innerHTML="";
			const iframe = document.createElement('iframe');
			iframe.src = `/pdfjs/web/viewer.html?file=${nextProps.url}`;
			iframe.width = '100%';
			iframe.height = '100%';
			iframe.style = 'border:none;';
			element.appendChild(iframe);
			this.iframe = iframe;

			const usedTime = 5;

			const _this = this;

			function onLoad() {
				if (_this.loadingTimer) {
					clearInterval(_this.loadingTimer);
				}
				_this.loading = false;
				if (_this.page != undefined) {
					_this.iframe.contentWindow.PDFViewerApplication.page = _this.page;
				}
				if (_this.props.recordEvent || _this.props.onLoad) {
					_this.timer = setInterval(() => {
						_this.props.recordEvent && _this.props.recordEvent({
							materialId: _this.props.id,
							endLocation: _this.iframe.contentWindow.PDFViewerApplication.page,
							usedTime: usedTime
						});
						_this.props.onLoad && _this.props.onLoad(_this.iframe.contentWindow)
					}, usedTime * 1000);
				}
			}

			_this.loadingTimer = setInterval(() => {
				if (
					_this.iframe &&
					_this.iframe.contentWindow &&
					_this.iframe.contentWindow.PDFViewerApplication &&
					_this.iframe.contentWindow.PDFViewerApplication.page
				) {
					onLoad();
				}
			}, 1000);
		}
	}

	render() {
		return <div ref={this.viewerRef} id="viewer" style={{ width: '100%', height: '100%' }} />;
	}
}
