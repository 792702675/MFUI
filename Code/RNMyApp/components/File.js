import React from 'react';
import {
    StyleSheet,
    View,
    Text,
    TouchableOpacity,
    ScrollView,
    Modal,
    Dimensions
} from 'react-native';
import { Button, ListRow, Overlay, Label, Menu } from 'teaset';
import { Actions } from 'react-native-router-flux';
import { Card, WhiteSpace, WingBlank, Grid, ActionSheet } from '@ant-design/react-native';
import AntDesign from 'react-native-vector-icons/AntDesign';
import Entypo from 'react-native-vector-icons/Entypo';
import Ionicons from 'react-native-vector-icons/Ionicons';
import MaterialIcons from 'react-native-vector-icons/MaterialIcons';
import ImagePicker from 'react-native-image-picker';
import ImageViewer from 'react-native-image-zoom-viewer';
import Http from "../url/feach";
import { NavigationBar } from 'teaset';
import Video from 'react-native-video';
const photoOptions = {
    title: '请选择',
    quality: 0.8,
    cancelButtonTitle: '取消',
    takePhotoButtonTitle: '拍照',
    chooseFromLibraryButtonTitle: '选择相册',
    allowsEditing: true,
    noData: false,
    storageOptions: {
        skipBackup: true,
        path: 'images'
    }
};
export class File extends React.Component {
    state = {
        filelist: [],
        bucketsname: '',
        navtitle: '',
        imagesurl: {},
        imagevisible: false,
        videourl: '',
        rate: 1,
        videovisible: false
    }
    selectPhotoTapped() {
        const options = {
            title: '选择图片',
            cancelButtonTitle: '取消',
            takePhotoButtonTitle: '拍照',
            chooseFromLibraryButtonTitle: '选择照片',
            // customButtons: [{ name: 'fb', title: 'Choose Photo from Facebook' },],
            cameraType: 'back',
            mediaType: 'photo',
            videoQuality: 'high',
            durationLimit: 10,
            maxWidth: 300,
            maxHeight: 300,
            quality: 0.8,
            angle: 0,
            allowsEditing: false,
            noData: false,
            storageOptions: { skipBackup: true }
        };
        ImagePicker.showImagePicker(options, (response) => {
            console.log('Response = ', response);
            if (response.didCancel) {
                console.log('User cancelled photo picker');
            } else if (response.error) {
                console.log('ImagePicker Error: ', response.error);
            } else if (response.customButton) {
                console.log('User tapped custom button: ', response.customButton);
            } else {
                // let source = {
                //     uri: response.uri
                // };
                Http.upload("/api/AliyunOss/CreateObject", response).then(data => {
                    if (data.success) {
                        alert('上传成功')
                    } else {
                        alert(data.message)
                    }

                    storage.load({
                        key: 'bucketsname',
                    }).then(ret => {
                        this.setState({
                            bucketsname: ret.bucketsname
                        })
                        storage.load({
                            key: 'roots',
                        }).then(res => {
                            console.log(res)
                            Http.get('/api/AliyunOss/GetObjectListOfDirectory', { bucketName: ret.bucketsname, root: res.roots }).then(data => {
                                console.log(data)
                                this.setState({
                                    filelist: data.result
                                })
                            })
                        })

                    })
                })

                // You can also display the image using data: 
                // let source = { uri: 'data:image/jpeg;base64,' + response.data }; 
                // this.setState({
                //     avatarSource: source
                // });
            }
        });
    }
    selectVideoTapped() {
        const options = {
            title: '选择视频',
            cancelButtonTitle: '取消',
            takePhotoButtonTitle: '录制视频',
            chooseFromLibraryButtonTitle: '选择视频',
            mediaType: 'video',
            videoQuality: 'medium'
        };
        ImagePicker.showImagePicker(options, (response) => {

            console.log('Response = ', response);
            if (response.didCancel) {
                console.log('User cancelled video picker');
            } else if (response.error) {
                console.log('ImagePicker Error: ', response.error);
            } else if (response.customButton) {
                console.log('User tapped custom button: ', response.customButton);
            } else {
                console.log(response)
                Http.upload("/api/AliyunOss/CreateObject", response).then(data => {
                    if (data.success) {
                        alert('上传成功')
                    } else {
                        alert(data.message)
                    }

                    storage.load({
                        key: 'bucketsname',
                    }).then(ret => {
                        this.setState({
                            bucketsname: ret.bucketsname
                        })
                        storage.load({
                            key: 'roots',
                        }).then(res => {
                            console.log(res)
                            Http.get('/api/AliyunOss/GetObjectListOfDirectory', { bucketName: ret.bucketsname, root: res.roots }).then(data => {
                                console.log(data)
                                this.setState({
                                    filelist: data.result
                                })
                            })
                        })

                    })
                })
                // this.setState({
                //     videoSource: response.uri
                // });
            }
        });
    }
    componentDidMount() {
        storage.load({
            key: 'bucketsname',
        }).then(ret => {
            this.setState({
                bucketsname: ret.bucketsname
            })
            storage.load({
                key: 'roots',
            }).then(res => {
                console.log(res)
                Http.get('/api/AliyunOss/GetObjectListOfDirectory', { bucketName: ret.bucketsname, root: res.roots }).then(data => {
                    console.log(data)
                    this.setState({
                        filelist: data.result
                    })
                })
            })

        })

    }
    render() {
        const { filelist, navtitle, bucketsname, imagesurl, imagevisible, videourl, videovisible } = this.state;
        // const { width, height } = Dimensions.get('window');
        let widths=Dimensions.get('window').width;
        let heights=Dimensions.get('window').height;
        let x = 260;
        let y = -250;
        let width = 200;
        let height = 300;
        let menulist = [{
            title: '图片', icon: <Entypo name='images' size={20} color='#fff' style={{ marginRight: 10 }} />, onPress: () => this.selectPhotoTapped()
        }, {
            title: '视频', icon: <AntDesign name='videocamera' size={20} color='#fff' style={{ marginRight: 10 }} />, onPress: () => this.selectVideoTapped()
        }, {
            title: <TouchableOpacity onPress={() => {Actions.audio()}}><Text style={{color:'#fff'}}>录音</Text></TouchableOpacity>, icon: <MaterialIcons name='audiotrack' size={20} color='#fff' style={{ marginRight: 10 }} />
        }]
        let overlayViewimages = (
            <Overlay.PullView side='bottom' modal={false}>
                <View style={{ backgroundColor: '#fff', minWidth: 300, minHeight: 260, justifyContent: 'center', alignItems: 'center' }}>
                    <Label type='title' size='xl' text='Pull Overlay' />
                </View>
            </Overlay.PullView>
        );
        return (
            <View style={styles.file}>
                <View style={{ height: 70 }}>
                    <NavigationBar title={navtitle ? navtitle : bucketsname} style={styles.navbar}
                        leftView={
                            <TouchableOpacity style={{ flexDirection: 'row', marginRight: 10 }} onPress={() => {
                                storage.load({
                                    key: 'roots',
                                }).then(ret => {
                                    if (ret.roots) {
                                        let arr = ret.roots.split('/');
                                        arr.splice(arr.length - 2, 1);
                                        Http.get('/api/AliyunOss/GetObjectListOfDirectory', { bucketName: bucketsname, root: arr.join('/') }).then(data => {
                                            console.log(data)
                                            this.setState({
                                                filelist: data.result
                                            })
                                        })
                                        storage.save({
                                            key: 'roots',
                                            data: {
                                                roots: arr.join('/')
                                            }
                                        })
                                    } else {
                                        Actions.index()
                                    }

                                })
                            }}>
                                <AntDesign name='left' size={28} style={{ marginLeft: 10 }} color='#fff' />
                            </TouchableOpacity>
                        }
                        rightView={
                            <TouchableOpacity style={{ flexDirection: 'row', marginRight: 10 }} onPress={() => {
                                Menu.show({ x, y, width, height }, menulist)
                            }}>
                                <Ionicons name='md-add' size={30} color='#fff' />
                            </TouchableOpacity>
                        }
                    />
                </View>
                <ScrollView>
                    <View style={styles.filebox}>
                        {
                            filelist ? filelist.map((item, index) => (
                                <TouchableOpacity style={{ width: 190 }} onPress={() => {
                                    if (!item.isFile) {
                                        storage.load({
                                            key: 'bucketsname',
                                        }).then(ret => {
                                            storage.load({
                                                key: 'roots',
                                            }).then(res => {
                                                console.log(res, "!!!!!!!!")
                                                let newroots = res.roots + item.name + '/'
                                                storage.save({
                                                    key: 'roots',
                                                    data: {
                                                        roots: newroots
                                                    }
                                                })
                                                Http.get('/api/AliyunOss/GetObjectListOfDirectory', { bucketName: ret.bucketsname, root: newroots }).then(data => {
                                                    console.log(data)
                                                    this.setState({
                                                        filelist: data.result
                                                    })
                                                })
                                            })

                                        })
                                    } else {
                                        if (item.isImage) {
                                            this.setState({
                                                imagesurl: item.url,
                                                imagevisible: true
                                            })
                                        } else if (
                                            item.extensionName == ".mp4"||
                                            item.extensionName == ".ogg"||
                                            item.extensionName == ".webm"||
                                            item.extensionName == ".MP4"||
                                            item.extensionName == ".MP3"||
                                            item.extensionName == ".mp3"||
                                            item.extensionName == ".wav"
                                        ) {
                                            this.setState({
                                                videourl: item.url,
                                                videovisible: true
                                            })
                                        }
                                    }
                                }}>
                                    <Card style={styles.listbox}>
                                        <Card.Body>
                                            <View style={styles.childbox}>
                                                {
                                                    item.isFile ?
                                                        item.isImage ?
                                                            <Entypo name='image' size={40} color='#000000' />
                                                            :
                                                            item.extensionName == ".pdf" ?
                                                                <AntDesign name='pdffile1' size={40} color='#000000' />
                                                                :
                                                                item.extensionName == ".pptx" ?
                                                                    <AntDesign name='pptfile1' size={40} color='#000000' />
                                                                    :
                                                                    item.extensionName == ".docx" ?
                                                                        <AntDesign name='wordfile1' size={40} color='#000000' />
                                                                        :
                                                                        item.extensionName == ".excel" ?
                                                                            <AntDesign name='exclefile1' size={40} color='#000000' />
                                                                            :
                                                                            (item.extensionName == ".mp4") ||
                                                                                (item.extensionName == ".ogg") ||
                                                                                (item.extensionName == ".webm") ||
                                                                                (item.extensionName == ".MP4") ?
                                                                                <Entypo name='video' size={40} color='#000000' />
                                                                                :

                                                                                (item.extensionName == ".MP3") ||
                                                                                    (item.extensionName == ".mp3") ||
                                                                                    (item.extensionName == ".wav") ?
                                                                                    <Entypo name='music' size={40} color='#000000' />
                                                                                    :
                                                                                    <AntDesign name='file1' size={40} color='#000000' />
                                                        :
                                                        <AntDesign name='folder1' size={40} color='#000000' />

                                                }

                                            </View>
                                            <View style={styles.childbox}>
                                                <Text numberOfLines={2} style={{ TextAlign: 'center' }}>{item.name}</Text>
                                            </View>
                                        </Card.Body>
                                    </Card>
                                    <WhiteSpace size="lg" />
                                </TouchableOpacity>
                            )) : null
                        }
                    </View>

                </ScrollView>
                <Modal visible={imagevisible} transparent={true}>
                    <ImageViewer
                        enableImageZoom={true}
                        saveToLocalByLongPress={true}
                        menuContext={{ "saveToLocal": "保存图片", "cancel": "取消" }}
                        onClick={() => this.setState({
                            imagesurl: '',
                            imagevisible: false
                        })} imageUrls={[{
                            url: imagesurl
                        }]} />
                </Modal>
                <Modal visible={videovisible} transparent={true}>
                    <TouchableOpacity style={styles.modalclose} onPress={()=>{
                        this.setState({
                            videovisible:false
                        })
                    }}><AntDesign name='close' size={28} style={{ marginLeft: 10 }} color='#fff' /></TouchableOpacity>
                    <Video
                        ref={(ref: Video) => { //方法对引用Video元素的ref引用进行操作
                            this.video = ref
                        }}
                        source={{ uri: videourl }}//设置视频源  
                        style={{ width:widths, height:heights,backgroundColor:'#000' }}
                        rate={1.0}
                        volume={1.0}
                        muted={false}
                        resizeMode={'contain'}
                        playWhenInactive={false}
                        playInBackground={false}
                        ignoreSilentSwitch={'ignore'}
                        progressUpdateInterval={250.0}

                    />
                </Modal>
            </View>
        );
    }
};

const styles = StyleSheet.create({
    file: {
        // display:'flex',
        // flexDirection:'column',
    },
    navbar: {
        // flex:1
    },
    filebox: {
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'space-around',
        flexWrap: 'wrap',
        paddingTop: 20,
        paddingBottom:100
    },
    listbox: {
        width: 190,
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        textAlign: 'center',
    },
    childbox: {
        height: 50,
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center',
        textAlign: 'right',
        padding: 20
    },
    modalclose:{
        position:'absolute',
        right:10,
        top:10,
        zIndex:1000
    },
});

export default File;
