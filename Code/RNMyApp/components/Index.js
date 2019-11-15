import React from 'react';
import {
    StyleSheet,
    View,
    Text,
    TouchableOpacity
} from 'react-native';
import { Button } from 'teaset';
import { Actions, Router, Scene } from 'react-native-router-flux';
import { Card, WhiteSpace, WingBlank } from '@ant-design/react-native'
import Http from "../url/feach";
import Icon from "react-native-vector-icons/Ionicons";
export class Index extends React.Component {
    state = {
        Buckets: ''
    }
    componentDidMount() {
        Http.get('/api/AliyunOss/GetBuckets', {}).then(res => {
            console.log(res.result)
            this.setState({
                Buckets: res.result
            })
        })

    }
    render() {
        const { Buckets } = this.state;
        return (
            <>
                <View style={{ paddingTop: 30 }}>
                    <WingBlank size="lg">
                        {
                            Buckets ? Buckets.map((item, index) => (
                                <TouchableOpacity onPress={() => {
                                    storage.save({
                                        key: 'bucketsname',
                                        data: {
                                            bucketsname: item.name
                                        }
                                    })
                                    storage.save({
                                        key: 'roots',
                                        data: {
                                            roots: ''
                                        }
                                    })
                                    Actions.file()
                                }}>
                                    <Card>
                                        <Card.Header
                                            title="存储库"
                                            thumbStyle={{ width: 30, height: 30 }}
                                            thumb="https://gw.alipayobjects.com/zos/rmsportal/MRhHctKOineMbKAZslML.jpg"
                                            extra="删除"
                                        />
                                        <Card.Body>
                                            <View style={{ height: 42 }}>
                                                <Text style={{ marginLeft: 16 }}>{item.name}</Text>
                                            </View>
                                        </Card.Body>
                                        <Card.Footer
                                            // content="footer content"
                                            extra={item.creationTime}
                                        />
                                    </Card>
                                    <WhiteSpace size="lg" />
                                </TouchableOpacity>
                            )) : null
                        }


                    </WingBlank>
                    {/* <Icon name='Entypo' size={30} color='#ff0000'></Icon> */}
                </View>

            </>
        );
    }
};

const styles = StyleSheet.create({

});

export default Index;
