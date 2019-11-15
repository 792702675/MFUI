import React from 'react';
import { connect } from 'dva';
import { call } from '../../api/apiUtil';
import { InputNumber, Icon } from 'antd';
class Adddeley extends React.Component {


    render() {
        console.log(this.props)
        const { value } = this.props;
        // let arr=[];
        // for(var i=0;i<result.length;i++){
        //     arr.push({image:{...result[i]}})
        // }
        return (
            <div>
                {
                    value && value.length !== 0 ?
                        value.map((a, b) => (<div><img src={a.image.url} style={{ width: 100 }} /><InputNumber defaultValue={a.delay} style={{ marginLeft: 10 }} min='0' onChange={(e) => {
                            value[b].delay = e;
                            this.props.onChange(value)
                        }} /><Icon type="delete" onClick={()=>{
                                value.splice(b,1)
                            this.props.onChange(value)
                        }}/></div>)) : null
                }
            </div>
        );
    }
}

export default Adddeley;
