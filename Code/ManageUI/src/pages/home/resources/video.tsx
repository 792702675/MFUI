import * as React from 'react';
import { connect } from 'dva';
import videojs from 'video.js';
class Video extends React.Component {
    componentDidMount() {
        let _this = this;
        const videoJsOptions = {
            controls: true,
            playbackRates: [0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4],
            autoplay: false,
            muted: false,
            width: '640px',
            languages: {
                en: {
                    Play: '播放',
                    Pause: '暂停',
                    Mute: '音量',
                    'Playback Rate': '倍速播放',
                    'Picture-in-Picture': '截图',
                    Fullscreen: '全屏',
                    'Non-Fullscreen': "取消全屏"
                }
            },
            sources: [
                {
                    src: this.props.item.url
                }
            ]
        };
       
        this.player = videojs(this.videoNode, {
            ...videoJsOptions }, () => {
            
        });
        // videojs.addLanguage('en', {
        //     Play: '播放',
        //     Pause: '暂停',
        //     Mute:'音量',
        //     'Playback Rate':'倍速播放',
        //     'Picture-in-Picture':'截图',
        //     Fullscreen:'全屏',
        //     'Non-Fullscreen':"取消全屏"
        // });
    }

    // destroy player on unmount
    componentWillUnmount() {
        if (this.player) {
            this.player.dispose();
        }
    }
    render() {
        return (
                    <div data-vjs-player>
                <video ref={(node) => (this.videoNode = node)} className="video-js" language='en'/>
                        </div>
        );
    }
}

Video = connect((state) => {
    return {
        ...state.oss
    };
})(Video);
export default Video;
