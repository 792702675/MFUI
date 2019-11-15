import React from 'react';
import { connect } from 'dva';
import { Modal } from 'antd';
import styles from '../../style/Animation.css';
let timerinter;
export default class Animation extends React.Component {

    constructor() {
        super();
        this.state = {

        };
    }
    componentDidMount() {
        const { items } = this.props;
        let _this = this;
        let timer = 50;
        timerinter=setInterval(function () {
            timer = timer + 50;
            for (var i = 0; i < items.length; i++) {
                if (timer >= _this.getdeledy(i)) {
                    _this.refs.donghua.src = items[i].image.url;
                    if (timer >= _this.getdeledy(items.length)) {
                        timer = 0
                    }
                }
            }

        }, 50)
    }
    componentWillUnmount(){
        clearInterval(timerinter)
    }
    getimage = (ele, arr) => {
        for (var i = 0; i < arr.length; i++) {

        }
    }
    getdeledy = (s) => {
        const { items } = this.props;
        let sub = 0;
        for (var i = 0; i < s; i++) {
            sub = sub + items[i].delay
        }
        return sub
    }
    render() {
        const { items } = this.props;
        return (
            <div style={{ transform: 'scale(0.8)',margin:'0 auto',textAlign:'center' }}>
                <img ref="donghua" src={items[0].image.url}/>
            </div>
        );
    }
}
