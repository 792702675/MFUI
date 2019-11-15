import * as React from 'react';
import { connect } from 'dva';
import { routerRedux, Link } from 'dva/router';
import { remoteUrl } from '../../../utils/url';
import { Card, Icon, Avatar, Checkbox, Row, Col, Button, Modal, Form, Breadcrumb, Input, Upload, message, Empty, Tree, Tooltip, Tag, Popover, notification, Spin, Select, Switch } from 'antd';
import styles from '../../../style/oss.css';
import Pdf from '../../../components/PDFViewer/PDFViewer';
import TagSelect from '@/components/System/TagSelect';
import Filter from '@/components/Filter/Filter';
import moment from 'moment';
import { call } from '../../../api/apiUtil';
import * as api from '../../../api/api';
import Video from './video'
const create = Form.create;
const { Meta } = Card;
const confirm = Modal.confirm;
const Dragger = Upload.Dragger;
const { TreeNode } = Tree;
const ButtonGroup = Button.Group;
const Search = Input.Search;
const CheckboxGroup = Checkbox.Group;
const { Option } = Select;
const InputGroup = Input.Group;
function File({ dispatch, resultall, form, iconcolor, deletefilevisible, updatevisible, tagOptionsprops,getTagsByGroupIds, wenjianjiavisible, dropDownList, startindex, visiblefolder, imgvisible, checkedbox, item, videovisible, leftbox, issearch, checklist, topbox, leixintg, shijian, chicun, visiblefixed, mingcheng, tags, addtag, copyvisible, buckets, searchvalue, biaoqianvisible, inputvalue, inputVisible, treeData, isLeaf, treelist, newnamevisible, pi, pdfvisible, url, fileitem, rootstrees, gogobuckets, modaltype, checkarrey, spinningloading }) {
    const { getFieldDecorator } = form;

    const formCol = {
        labelCol: { span: 8 },
        wrapperCol: { span: 12 }
    };
    const formCol2 = {
        labelCol: { span: 4 },
        wrapperCol: { span: 10 }
    };
    let handleCancel = () => {
        dispatch({
            type: 'oss/setState',
            payload: {
                visiblefolder: false
            }
        })
        form.resetFields();
    }
    let handleSubmit = e => {
        e.preventDefault();
        form.validateFields((err, values) => {
            console.log(values)
            // if (!err) {
            dispatch({
                type: 'oss/CreateFolder',
                payload: {
                    bucketName: window.sessionStorage.getItem('bucketName'),
                    folder: window.sessionStorage.getItem('root') + values.folder,
                    tagNames: values.tagsNames
                }
            })
            form.resetFields();
            // }
        });
    }
    let filter = [
        {
            name: 'name',
            displayName: '文件名/文件夹名'
        }
    ];
    filter.push({
        name: 'tagNames',
        displayName: '标签',
        type: 'select',
        selectOptions: [],
        props: { mode: 'tags' }
    }, {
            name: 'extensionNames',
            displayName: '扩展名',
            type: 'select',
            selectOptions: [],
            props: { mode: 'tags' }
        })
    const props = {
        name: 'file',
        multiple: true,
        beforeUpload() { },
        action: `${remoteUrl}/api/AliyunOss/CreateObject?bucketName=${window.sessionStorage.getItem('bucketName')}&path=${window.sessionStorage.getItem('root')? window.sessionStorage.getItem('root') : ''}${window.sessionStorage.getItem('tagurl')? window.sessionStorage.getItem('tagurl'):''}`,
        headers: {
            authorization: 'Bearer ' + window.localStorage.getItem('token')
        },
        showUploadList: { showRemoveIcon: false },
        onChange(info) {
            const status = info.file.status;
            if (status !== 'uploading') {
            }
            if (status === 'done') {
                dispatch({
                    type: 'oss/getObjectListOfDirectory',
                    payload: {
                        bucketName: window.sessionStorage.getItem('bucketName'),
                        root: window.sessionStorage.getItem('root')
                    }
                })
                message.success(`${info.file.name} 上传成功.`);
            } else if (status === 'error') {
                message.error(`${info.file.name} 上传失败.`);
            }
        },
    };
    const props2 = {
        name: 'file',
        multiple: true,
        action: `${remoteUrl}/api/AliyunOss/CreateObject?bucketName=${window.sessionStorage.getItem('bucketName')}&path=${window.sessionStorage.getItem('root') !== "" ? window.sessionStorage.getItem('root') : ''}${window.sessionStorage.getItem('tagurl') !== '' ? window.sessionStorage.getItem('tagurl') : ''}`,
        headers: {
            authorization: 'Bearer ' + window.localStorage.getItem('token')
        },
        data(file) { return { filepath: file.webkitRelativePath } },
        showUploadList: { showRemoveIcon: false },
        onChange(info) {
            console.log(info)
            const status = info.file.status;
            if (status !== 'uploading') {
            }
            if (status === 'done') {
                dispatch({
                    type: 'oss/getObjectListOfDirectory',
                    payload: {
                        bucketName: window.sessionStorage.getItem('bucketName'),
                        root: window.sessionStorage.getItem('root')
                    }
                })
                message.success(`${info.file.name} 上传成功.`);
            } else if (status === 'error') {
                message.error(`${info.file.name} 上传失败.`);
            }
        },
    };
    let onVChange = e => {
        if (e.target.checked) {
            // e.target.isfile ? 
            checkarrey.push(e.target.value)
            //  : checkarrey.push(e.target.value + '/')
        } else {
            // e.target.isfile ?
            checkarrey = checkarrey.filter(v => v != e.target.value)
            //  : checkarrey = checkarrey.filter(v => v != e.target.value + '/');
        }
        dispatch({
            type: "oss/setState",
            payload: {
                checkarrey: [...checkarrey]
            }
        })
    }
    let rightclick = (e, item) => {
        if (e.button == 2) {
            e.preventDefault();
            dispatch({
                type: "oss/setState",
                payload: {
                    fileitem: item,
                    visiblefixed: true,
                    leftbox: e.clientX + 'px',
                    topbox: e.clientY + 'px'
                }
            })
        }
        document.onclick = function () {
            dispatch({
                type: "oss/setState",
                payload: {
                    visiblefixed: false,
                }
            })
            document.onclick = null
        }
    }


    let onLoadData = treeNode =>
        new Promise(resolve => {
            if (treeNode.props.children) {
                resolve();
                return;
            }
            let roots;
            for (var i = 1; i < treeNode.props.eventKey.split('-').length; i++) {
                roots ? roots = roots + '/' + treeNode.props.eventKey.split('-')[i] + '/' : roots = treeNode.props.eventKey.split('-')[i]
            }
            dispatch({
                type: 'oss/getObjectListOfDirectory',
                payload: {
                    bucketName: treeNode.props.eventKey.split('-')[0],
                    root: roots,
                    directoryOnly: true
                },
                treeNode: treeNode,
                callback: () => {
                    resolve();
                }
            })
        });
    let renderTreeNodes = data =>
        data.map(item => {
            if (item.children) {
                return (
                    <TreeNode title={item.title} key={item.key} dataRef={item} isLeaf={isLeaf}>
                        {renderTreeNodes(item.children)}
                    </TreeNode>
                );
            }
            return <TreeNode {...item} dataRef={item} />;
        });
    let onSelect = (keys, event) => {
        let rootstree;
        for (var i = 1; i < keys[0].split('-').length; i++) {
            rootstree ? rootstree = rootstree + '/' + keys[0].split('-')[i] : rootstree = keys[0].split('-')[i]
        }
        dispatch({
            type: "oss/setState",
            payload: {
                gogobuckets: keys[0].split('-')[0],
                rootstrees: rootstree ? rootstree + '/' : null
            }
        })
    };
    let createtagsurl = e => {
        let sub = '';
        e.forEach(el => {
            sub = sub + "&tagNames=" + el
        });
        window.sessionStorage.setItem('tagNames', e)
        window.sessionStorage.setItem('tagurl', sub)
    }
    let handleSubmitnewname = e => {
        e.preventDefault();
        form.validateFields((err, values) => {
            console.log(values)
            let arr = fileitem.key.split('/');
            let swaggers;
            if (fileitem.isFile) {
                arr.splice(arr.length - 1, 1);
                swaggers = arr.join('/') ? arr.join('/') + "/" + values.newname + values.houzhui : values.newname + values.houzhui
            } else {
                arr.splice(arr.length - 2, 1);
                swaggers = arr.join('/') ? arr.join('/') + values.newname + "/" : values.newname + "/"
            }
            dispatch({
                type: "oss/rename",
                payload: {
                    sourceBucketName: window.sessionStorage.getItem('bucketName'),
                    sourceKey: fileitem.key,
                    destinationBucketName: window.sessionStorage.getItem('bucketName'),
                    destinationKey: swaggers,
                }
            })
        });
        form.resetFields();
    }
    let handleSubmittaglist = e => {
        e.preventDefault();
        form.validateFields((err, values) => {
            let source = [];
            for (var i = 0; i < checkarrey.length; i++) {
                source.push({ bucketName: window.sessionStorage.getItem('bucketName'), key: checkarrey[i] })
            }
            if (addtag == "add") {
                dispatch({
                    type: "oss/batchAddTag",
                    payload: {
                        source: source,
                        tagNames: values.taglist
                    }
                })
            } else if (addtag == 'sub') {
                dispatch({
                    type: "oss/batchSubTag",
                    payload: {
                        source: source,
                        tagNames: values.taglist
                    }
                })
            } else {
                dispatch({
                    type: "oss/updateTag",
                    payload: {
                        bucketName: window.sessionStorage.getItem('bucketName'),
                        key: fileitem.key,
                        tagNames: values.taglist,
                        applyAllChild: values.applyAllChild
                    }
                })
            }
            form.resetFields();
        });
    }
    


    return (
        <div style={{ height: '100%' }}>

            <Row style={{ marginTop: 20 }}>
                <Col xxl={3} xl={4} lg={6} md={12}>
                    <Button type="primary" onClick={() => {
                        if (issearch) {
                            dispatch({
                                type: "oss/getObjectListOfDirectory",
                                payload: {
                                    bucketName: window.sessionStorage.getItem('bucketName'),
                                    root: window.sessionStorage.getItem('root')
                                }
                            })
                            dispatch({
                                type: "oss/setState",
                                payload: {
                                    issearch: false,
                                    mingcheng: true,
                                    leixintg: true,
                                    shijian: true,
                                    chicun: true,
                                    startindex: 0
                                }
                            })
                        } else {
                            let roots = window.sessionStorage.getItem('root');
                            if (!roots) {
                                dispatch(routerRedux.push('/home/resources'))
                                return
                            }
                            let arr = roots.split('/');
                            arr.splice(arr.length - 2, 1);
                            window.sessionStorage.setItem('root', arr.join('/'))
                            dispatch({
                                type: "oss/getObjectListOfDirectory",
                                payload: {
                                    bucketName: window.sessionStorage.getItem('bucketName'),
                                    root: window.sessionStorage.getItem('root')
                                }
                            })
                            call({
                                method: new api.AliyunOssApi().getTags,
                                payload: {
                                    bucketName: window.sessionStorage.getItem('bucketName'),
                                    key: window.sessionStorage.getItem('root'),
                                }
                            }).then(({ result }) => {
                                form.setFieldsValue({ 'tagsGroup': result, 'tagsGroup2': result })
                                createtagsurl(result)
                            });
                        }



                    }}>
                        <Icon type="left" />
                        返回
                </Button>
                </Col>
                <Col md={12} style={{
                    padding: 5,
                    background: '#f8f8f8'
                }}>
                    <Breadcrumb>
                        <Breadcrumb.Item> <a onClick={() => {
                            dispatch({
                                type: "oss/getObjectListOfDirectory",
                                payload: {
                                    bucketName: window.sessionStorage.getItem('bucketName'),
                                    root: ''
                                }
                            })
                            window.sessionStorage.setItem('root', '')
                        }}>{window.sessionStorage.getItem('bucketName')}</a></Breadcrumb.Item>
                        {
                            window.sessionStorage.getItem('root').split('/').map((v, s) => (v ? <Breadcrumb.Item>
                                <a onClick={() => {
                                    let sub = window.sessionStorage.getItem('root').split('/');
                                    let obj = '';
                                    for (var i = 0; i < s + 1; i++) {
                                        obj ? obj = obj + '/' + sub[i] : obj = sub[i]
                                    }
                                    dispatch({
                                        type: "oss/getObjectListOfDirectory",
                                        payload: {
                                            bucketName: window.sessionStorage.getItem('bucketName'),
                                            root: obj + '/'
                                        }
                                    })
                                    window.sessionStorage.setItem('root', obj + '/')
                                }}>{v}</a>
                            </Breadcrumb.Item> : null))
                        }
                    </Breadcrumb>
                    {/* <Input disabled={true} value={window.sessionStorage.getItem('root') ? window.sessionStorage.getItem('bucketName') + '/' + window.sessionStorage.getItem('root') : window.sessionStorage.getItem('bucketName')+'/'} style={{ color: "red" }} /> */}
                </Col>
            </Row>
            <Row gutter={16} style={{ marginTop: '20px' }}>
                <ButtonGroup>
                    <Button size="large" onClick={() => {dispatch({
                        type: "oss/setState",
                        payload: {
                            visiblefolder: true
                        }
                    })
                        call({
                            method: new api.AliyunOssApi().getTags,
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                key: window.sessionStorage.getItem('root'),
                            }
                        }).then(({ result }) => {
                            form.setFieldsValue({ 'tagsNames': result })
                            createtagsurl(result)
                        });
                    }}>
                        <Icon type="plus" />
                        新建文件夹
                    </Button>
                    <Button size="large" onClick={() => {
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                updatevisible: true
                            }
                        })
                        call({
                            method: new api.AliyunOssApi().getTags,
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                key: window.sessionStorage.getItem('root'),
                            }
                        }).then(({ result }) => {
                            form.setFieldsValue({ 'tagsGroup': result, 'tagsGroup2': result })
                            createtagsurl(result)
                        });
                    }}><Icon type="upload" />上传文件</Button>
                    <Button size="large" onClick={() => {
                        dispatch({ type: "oss/setState", payload: { wenjianjiavisible: true } })
                        call({
                            method: new api.AliyunOssApi().getTags,
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                key: window.sessionStorage.getItem('root'),
                            }
                        }).then(({ result }) => {
                            form.setFieldsValue({ 'tagsGroup': result, 'tagsGroup2': result })
                            createtagsurl(result)
                        });
                    }}><Icon type="upload" />上传文件夹</Button>
                    <Button size="large" onClick={() => {
                        for (var i = 0; i < window.sessionStorage.getItem('buckets').split(',').length; i++) {
                            treeData.push({ title: window.sessionStorage.getItem('buckets').split(',')[i], key: window.sessionStorage.getItem('buckets').split(',')[i] })
                        }
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                copyvisible: true,
                                treeData: [...treeData],
                                modaltype: 'copy',
                                pi: true
                            }
                        })
                    }}>
                        <Icon type="copy" />
                        批量复制到...
                    </Button>
                    <Button size="large" onClick={() => {
                        for (var i = 0; i < window.sessionStorage.getItem('buckets').split(',').length; i++) {
                            treeData.push({ title: window.sessionStorage.getItem('buckets').split(',')[i], key: window.sessionStorage.getItem('buckets').split(',')[i] })
                        }
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                copyvisible: true,
                                treeData: [...treeData],
                                modaltype: 'move',
                                pi: true
                            }
                        })
                    }}>
                        <Icon type="scissor" />
                        批量移动到...
                    </Button>
                    <Button size="large" onClick={() => dispatch({
                        type: "oss/setState",
                        payload: {
                            biaoqianvisible: true,
                            addtag: 'add'
                        }
                    })}><Icon type="tag" />批量增加标签</Button>
                    <Button size="large" onClick={() => dispatch({
                        type: "oss/setState",
                        payload: {
                            biaoqianvisible: true,
                            addtag: 'sub'
                        }
                    })}><Icon type="delete" />批量删除标签</Button>
                    <Button size="large" onClick={() => confirm({
                        title: '警告',
                        content: '你确定要删除吗？',
                        okText: "确定",
                        cancelText: "取消",
                        onOk() {
                            for (var i = 0; i < checkarrey.length; i++) {
                                if (checkarrey[i].charAt(checkarrey[i].length - 1) == '/') {
                                    message.info('文件夹不能进行批量删除操作')
                                    return
                                }
                            }
                            dispatch({
                                type: "oss/DeleteObjects",
                                payload: {
                                    bucketName: window.sessionStorage.getItem('bucketName'),
                                    fileName: checkarrey
                                }
                            })

                        },
                        onCancel() { },
                    })}><Icon type="delete" />批量删除</Button>
                </ButtonGroup>
            </Row>
            <Row gutter={16} style={{ marginTop: '20px' }} type="flex" justify="start">
                <Col>
                    <Filter
                        onSearch={(value) => {
                            dispatch({
                                type: "oss/search",
                                payload: {
                                    bucketName: window.sessionStorage.getItem('bucketName'),
                                    root: window.sessionStorage.getItem('root'),
                                    ...value
                                }
                            })
                            dispatch({
                                type: 'oss/setState',
                                payload: {
                                    searchvalue: value,
                                    issearch: true
                                }
                            })
                        }}
                        filters={filter} onFilterClear={() => {
                            dispatch({
                                type: 'oss/getObjectListOfDirectory',
                                payload: {
                                    bucketName: window.sessionStorage.getItem('bucketName'),
                                    root: window.sessionStorage.getItem('root')
                                }
                            });
                            dispatch({
                                type: "oss/setState",
                                payload: {
                                    issearch: false,
                                    mingcheng: true,
                                    leixintg: true,
                                    shijian: true,
                                    chicun: true
                                }
                            })
                        }} />
                </Col>
                <Col>
                </Col>
            </Row>
            <Row>
                <ButtonGroup>
                    <Button onClick={() => {
                        dispatch({
                            type: issearch ? 'oss/search' : 'oss/getObjectListOfDirectory',
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                root: window.sessionStorage.getItem('root'),
                                sorting: mingcheng ? 'name' : 'name desc',
                                ...searchvalue
                            }
                        })
                        dispatch({
                            type: 'oss/setState',
                            payload: {
                                mingcheng: !mingcheng
                            }
                        })
                    }}>名称{mingcheng ? <Icon type="arrow-down" /> : <Icon type="arrow-up" />}</Button>
                    <Button onClick={() => {
                        dispatch({
                            type: issearch ? 'oss/search' : 'oss/getObjectListOfDirectory',
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                root: window.sessionStorage.getItem('root'),
                                sorting: leixintg ? 'extensionName' : 'extensionName desc',
                                ...searchvalue
                            }
                        })
                        dispatch({
                            type: 'oss/setState',
                            payload: {
                                leixintg: !leixintg
                            }
                        })
                    }}>类型{leixintg ? <Icon type="arrow-down" /> : <Icon type="arrow-up" />}</Button>
                    <Button onClick={() => {
                        dispatch({
                            type: issearch ? 'oss/search' : 'oss/getObjectListOfDirectory',
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                root: window.sessionStorage.getItem('root'),
                                sorting: shijian ? 'lastModified' : 'lastModified desc',
                                ...searchvalue
                            }
                        })
                        dispatch({
                            type: 'oss/setState',
                            payload: {
                                shijian: !shijian
                            }
                        })
                    }}>时间{shijian ? <Icon type="arrow-down" /> : <Icon type="arrow-up" />}</Button>
                    <Button onClick={() => {
                        dispatch({
                            type: issearch ? 'oss/search' : 'oss/getObjectListOfDirectory',
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                root: window.sessionStorage.getItem('root'),
                                sorting: chicun ? 'size' : 'size desc',
                                ...searchvalue
                            }
                        })
                        dispatch({
                            type: 'oss/setState',
                            payload: {
                                chicun: !chicun
                            }
                        })
                    }}>尺寸{chicun ? <Icon type="arrow-down" /> : <Icon type="arrow-up" />}</Button>
                    <Button><Checkbox checked={checkedbox} onChange={(e) => {
                        if (e.target.checked) {
                            let arrs = [];
                            for (let i = 0; i < resultall.length; i++) {
                                arrs.push(resultall[i].key)
                            }
                            dispatch({
                                type: "oss/setState",
                                payload: {
                                    checkarrey: arrs,
                                    checkedbox: true,
                                    startindex
                                }
                            })
                        } else {
                            dispatch({
                                type: "oss/setState",
                                payload: {
                                    checkarrey: [],
                                    checkedbox: false,
                                    startindex
                                }
                            })
                        }

                    }}>全选</Checkbox></Button>
                </ButtonGroup>
            </Row>
            <CheckboxGroup style={{ display: 'block' }} value={checkarrey} onChange={(checkarrey) => {
                dispatch({
                    type: "oss/setState",
                    payload: {
                        checkarrey
                    }
                })
            }}>
                <Row gutter={16} style={{ marginTop: '20px' }}>

                    {
                        resultall && resultall.length != 0 ?
                            resultall.map((item, index) => (
                                <Col xxl={4} lg={6} key={"absp" + index} style={{ marginBottom: '20px' }} onContextMenu={(e) => rightclick(e, item)}>
                                    <Card
                                        // extra={}
                                        key={item.key + "ss"}
                                        hoverable
                                        style={{ userSelect: 'none' }}
                                        // title={}
                                        // headStyle={{ padding: "0px 20px" }}
                                        bodyStyle={{ padding: "15px 0px" }}
                                        onDoubleClick={() => {
                                            if (item.isFile) {
                                                if (item.isImage) {
                                                    dispatch({
                                                        type: "oss/setState",
                                                        payload: {
                                                            imgvisible: true,
                                                            item: item,
                                                            spinningloading: true,
                                                            startindex: 0
                                                        }
                                                    })
                                                } else if (item.extensionName == ".pdf") {
                                                    dispatch({
                                                        type: "oss/setState",
                                                        payload: {
                                                            pdfvisible: true,
                                                            url: item.url,
                                                            startindex: 0
                                                        }
                                                    })
                                                } else if (
                                                    (item.extensionName == ".mp4") || 
                                                    (item.extensionName == ".ogg") || 
                                                    (item.extensionName == ".webm") || 
                                                    (item.extensionName == ".MP4") ||
                                                    (item.extensionName == ".MP3") || 
                                                    (item.extensionName == ".mp3") || 
                                                    (item.extensionName == ".wav")) {
                                                    dispatch({
                                                        type: "oss/setState",
                                                        payload: {
                                                            videovisible: true,
                                                            item: item,
                                                            startindex: 0
                                                        }
                                                    })
                                                } else if (item.officeViewUrl){
                                                    window.open(item.officeViewUrl)
                                                } else {
                                                    notification['error']({
                                                        message: "失败",
                                                        description: <span>尚不支持打开此格式文件</span>
                                                    })
                                                }
                                            } else {
                                                dispatch({
                                                    type: "oss/getObjectListOfDirectory",
                                                    payload: {
                                                        bucketName: window.sessionStorage.getItem('bucketName'),
                                                        root: item.key
                                                    }
                                                })
                                                dispatch({
                                                    type: "oss/setState",
                                                    payload: {
                                                        checkarrey: [],
                                                        startindex: 0
                                                    }
                                                })
                                                window.sessionStorage.setItem('root', item.key)
                                                call({
                                                    method: new api.AliyunOssApi().getTags,
                                                    payload: {
                                                        bucketName: window.sessionStorage.getItem('bucketName'),
                                                        key: window.sessionStorage.getItem('root'),
                                                    }
                                                }).then(({ result }) => {
                                                    form.setFieldsValue({ 'tagsGroup': result, 'tagsGroup2': result })
                                                    createtagsurl(result)
                                                });
                                            }

                                        }}
                                        onClick={(e) => {
                                            if (e.shiftKey == 1) {
                                                let arrs = [];
                                                if (startindex > index) {
                                                    for (let i = index; i < startindex + 1; i++) {
                                                        arrs.push(resultall[i].key)
                                                    }
                                                    dispatch({
                                                        type: "oss/setState",
                                                        payload: {
                                                            checkarrey: arrs,
                                                            checkedbox: false
                                                        }
                                                    })
                                                } else {
                                                    for (let i = startindex; i < index + 1; i++) {
                                                        arrs.push(resultall[i].key)
                                                    }
                                                    dispatch({
                                                        type: "oss/setState",
                                                        payload: {
                                                            checkarrey: arrs,
                                                            checkedbox: false
                                                        }
                                                    })
                                                }
                                            } else if (e.ctrlKey) {
                                                checkarrey.indexOf(item.key) > -1 ? checkarrey = checkarrey.filter(v => v != item.key) : checkarrey.push(item.key)
                                                dispatch({
                                                    type: "oss/setState",
                                                    payload: {
                                                        checkarrey: [...checkarrey],
                                                        checkedbox: false,
                                                        startindex: index
                                                    }
                                                })
                                            } else {
                                                dispatch({
                                                    type: "oss/setState",
                                                    payload: {
                                                        checkarrey: [item.key],
                                                        checkedbox: false,
                                                        startindex: index
                                                    }
                                                })
                                            }
                                        }}
                                    >
                                        <div className={styles.cardcontent}>
                                            <Checkbox value={item.key} style={{ position: 'absolute', right: 10, top: 10 }} />
                                            <div className={styles.cardtop}>

                                                <div className={styles.cardimg}>
                                                    {item.isFile ? item.isImage ? <img src={item.icon} style={{ maxWidth: '60px', maxHeight: '60px' }} /> : <Icon type={item.icon} style={{ fontSize: 60, color: iconcolor }} /> : <Icon type="folder-open" theme="filled" style={{ fontSize: '60px', color: '#f7d675' }} />}
                                                </div>
                                                <div className={styles.cardbottom}>
                                                    <Popover content={
                                                        <div style={{ cursor: 'pointer' }}>
                                                            {item.name}
                                                        </div>
                                                    }>
                                                        <div className={styles.filenamestyle}>{item.name}</div>
                                                    </Popover>
                                                    <header className={styles.cardtitle}>

                                                    </header>
                                                    <div className={styles.cardsub}>
                                                        <div className={styles.cardname}>
                                                            <div className={styles.muluname}>{item.isFile ? item._Size : "目录"}</div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div className={styles.cardtags}>
                                                {
                                                    item.tagNames.map((s, v) => (<Tag color="#2db7f5" key={s + v}>{s}</Tag>))
                                                }
                                            </div>
                                            {
                                                item.tagNames.length > 0 ? <Popover content={
                                                    <div style={{ width: 250 }}>
                                                        {item.tagNames.map((s, v) => (<Tag color="#2db7f5" style={{ marginBottom: 5 }} key={s}>{s}</Tag>))}
                                                    </div>
                                                }><div style={{ position: 'absolute', right: '3%', bottom: '5px', fontSize: 30, cursor: 'pointer' }}>...</div></Popover> : null
                                            }
                                        </div>
                                    </Card>
                                </Col>
                            ))
                            : <Empty />
                    }

                </Row>
            </CheckboxGroup>
            <Modal
                title="文件夹"
                visible={visiblefolder}
                onOk={handleSubmit}
                onCancel={handleCancel}
                footer={[<Button onClick={handleCancel}>
                    关闭
                    </Button>,
                <Button type="primary" onClick={handleSubmit}>
                    确定
            </Button>]}
            >
                <Form onSubmit={handleSubmit}>
                    <Form.Item label="标签组" {...formCol2}>
                        <Select style={{ width: 120 }} key="dsafsfs" onSelect={(e) => {
                            call({
                                method: new api.SysFunApi().getTagsByGroupId,
                                payload: {
                                    id: e
                                }
                            }).then(({ result }) => {
                                form.setFieldsValue({ 'tagsGroup': result, 'tagsGroup2': result })
                                dispatch({
                                    type:"oss/setState",
                                    payload:{
                                        tagOptionsprops:result
                                    }
                                })
                                createtagsurl(result)
                            });
                        }}>
                            {
                                dropDownList ? dropDownList.map((s, v) => (<Option value={s.value} key={s.name}>{s.name}</Option>)) : null
                            }
                        </Select>
                    </Form.Item>
                    <Form.Item>
                        {getFieldDecorator('tagsNames', {
                            // initialValue: window.sessionStorage.getItem('tagNames') ? window.sessionStorage.getItem('tagNames').split(',') : null
                        })(
                            <TagSelect tagOptionsprops={tagOptionsprops}  onChange={(e) => createtagsurl(e)} />
                        )}
                    </Form.Item>
                    <Form.Item label="名称" {...formCol}>
                        {getFieldDecorator('folder', {
                            rules: [{ required: true, message: '请输入您的文件名!' }],
                        })(
                            <Input
                                placeholder="文件夹名"
                            />,
                        )}
                    </Form.Item>
                    <Form.Item label="目录命名规范：" {...formCol}>
                        <div>1：不允许使用表情符，请使用符合要求得utf-8字符。</div>
                        <div>2: / 用于分割路径，可快速创建子目录，但，不要以 / 打头，不要出现连续得 / </div>
                        <div>3：不允许出现 ... 得子目录。</div>
                        <div>4：总长度控制在1-254个字符。</div>
                    </Form.Item>
                </Form>
            </Modal>
            <Modal
                title={item ?  window.sessionStorage.getItem('root') + item.name : null}
                visible={imgvisible}
                onCancel={() => dispatch({
                    type: "oss/setState",
                    payload: {
                        imgvisible: false,
                        item: ''
                    }
                })}
                footer={[
                    <Button key="back" onClick={() => dispatch({
                        type: "oss/setState",
                        payload: {
                            imgvisible: false,
                            item: ''
                        }
                    })}>
                        关闭
                    </Button>,
                    <Button type="primary">
                        <a target="_blank" href={item.url}>查看大图</a>
                    </Button>
                ]}
            >
                <Spin spinning={spinningloading}><img src={item ? item.url : null} onLoad={() => dispatch({
                    type: "oss/setState",
                    payload: {
                        spinningloading: false
                    }
                })} style={{ display: "block", maxWidth: '450px', maxHeight: '500px', margin: '0 auto' }} /></Spin>
            </Modal>
            {
                videovisible ? <Modal
                    title={item ? window.sessionStorage.getItem('root') + '/' + item.name : null}
                    visible={videovisible}
                    width={700}
                    onCancel={() => dispatch({
                        type: "oss/setState",
                        payload: {
                            videovisible: false,
                            item: ''
                        }
                    })}
                    footer={[
                        <Button key="back" onClick={() => dispatch({
                            type: "oss/setState",
                            payload: {
                                videovisible: false,
                                item: ''
                            }
                        })}>
                            关闭
                    </Button>
                    ]}
                >
                        <Video item={item} style={{width:'640px'}}/>
                </Modal> : null
            }

            {copyvisible ? <Modal
                title={"请选择你的目标目录"}
                visible={true}
                onCancel={() => {
                    dispatch({
                        type: "oss/setState",
                        payload: {
                            copyvisible: false,
                            treeData: [],
                            isLeaf: false
                        }
                    })
                }}
                footer={[
                    <Button onClick={() => dispatch({
                        type: "oss/setState",
                        payload: {
                            copyvisible: false,
                            treeData: [],
                            isLeaf: false
                        }
                    })}>
                        取消
                    </Button>,
                    <Button type="primary" onClick={() => {
                        if (pi) {
                            let source = [];
                            for (var i = 0; i < checkarrey.length; i++) {
                                source.push({ bucketName: window.sessionStorage.getItem('bucketName'), key: checkarrey[i] })
                            }
                            dispatch({
                                type: modaltype == "copy" ? "oss/copyBatch" : "oss/moveBatch",
                                payload: {
                                    "source": source,
                                    "destination": {
                                        "bucketName": gogobuckets,
                                        "key": rootstrees
                                    }
                                }
                            })
                        } else {
                            dispatch({
                                type: modaltype == "copy" ? "oss/copy" : "oss/move",
                                payload: {
                                    sourceBucketName: window.sessionStorage.getItem('bucketName'),
                                    sourceKey: fileitem.key,
                                    destinationBucketName: gogobuckets,
                                    destinationKey: rootstrees
                                }
                            })
                        }
                    }}>
                        确定
                    </Button>
                ]}
            >

                <Tree loadData={onLoadData} onSelect={onSelect}>{renderTreeNodes(treeData)}</Tree>
            </Modal> : null
            }
            {
                visiblefixed ? <div className={styles.rightclickbox} style={{ width: 150, minHeight: 100, position: 'absolute', left: leftbox, top: topbox }}>
                    <div onClick={() => {
                        for (var i = 0; i < window.sessionStorage.getItem('buckets').split(',').length; i++) {
                            treeData.push({ title: window.sessionStorage.getItem('buckets').split(',')[i], key: window.sessionStorage.getItem('buckets').split(',')[i] })
                        }
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                copyvisible: true,
                                treeData: [...treeData],
                                modaltype: 'copy',
                                pi: false
                            }
                        })
                    }}><Icon type="copy" style={{ marginRight: 10, color: iconcolor }} />复制到...</div>
                    <div onClick={() => {
                        for (var i = 0; i < window.sessionStorage.getItem('buckets').split(',').length; i++) {
                            treeData.push({ title: window.sessionStorage.getItem('buckets').split(',')[i], key: window.sessionStorage.getItem('buckets').split(',')[i] })
                        }
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                copyvisible: true,
                                treeData: [...treeData],
                                modaltype: 'move',
                                pi: false
                            }
                        })
                    }}><Icon type="scissor" style={{ marginRight: 10, color: iconcolor }} />移动到...</div>
                    <div onClick={() => {
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                newnamevisible: true
                            }
                        })
                        // console.log(fileitem.name.split('/'))
                        form.setFieldsValue({ 'newname': fileitem.name.split('.')[0], 'houzhui': fileitem.extensionName })
                    }}><Icon type="delete" style={{ marginRight: 10, color: iconcolor }} />重命名</div>
                    <div onClick={() => {
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                biaoqianvisible: true,
                                addtag: ''
                            }
                        })
                        form.setFieldsValue({ 'taglist': fileitem.tagNames })
                    }}><Icon type="edit" style={{ marginRight: 10, color: iconcolor }} />更新标签</div>
                    {
                        fileitem.isFile ? <div onClick={() => {
                            let arrkey = fileitem.key.split('/');
                            arrkey.splice(arrkey.length - 1, 1);
                            window.sessionStorage.setItem('root', arrkey.join('/') + '/')
                            dispatch({
                                type: "oss/getObjectListOfDirectory",
                                payload: {
                                    bucketName: window.sessionStorage.getItem('bucketName'),
                                    root: arrkey.join('/') + '/'
                                }
                            })
                        }}><Icon type="folder" style={{ marginRight: 10, color: iconcolor }} />打开所在目录</div> : null
                    }
                    <div onClick={() => dispatch({
                        type: "oss/setState",
                        payload: {
                            deletefilevisible: true
                        }
                    })} ><Icon type="delete" style={{ marginRight: 10, color: iconcolor }} />删除</div>
                    {
                        fileitem.isFile ? <div onClick={() =>{
                            window.open(fileitem.url)
                        }}><Icon type="folder" style={{ marginRight: 10, color: iconcolor }}/>下载</div> : null
                    }
                </div> : null
            }
            <Modal
                title={"PDF预览"}
                visible={pdfvisible}
                width={1200}
                height={800}
                onCancel={() => dispatch({
                    type: "oss/setState",
                    payload: {
                        pdfvisible: false
                    }
                })}
                footer={[
                    <Button key="back" onClick={() => dispatch({
                        type: "oss/setState",
                        payload: {
                            pdfvisible: false
                        }
                    })}>
                        关闭
                    </Button>
                ]}
            >
                <div style={{ height: 800 }}><Pdf url={url} key={url} /></div>
            </Modal>
            <Modal
                title={"重命名"}
                visible={newnamevisible}
                onOk={handleSubmitnewname}
                onCancel={() => {
                    dispatch({
                        type: "oss/setState",
                        payload: {
                            newnamevisible: false
                        }
                    })
                    form.resetFields();
                }}
                footer={[
                    <Button key="back" onClick={() => {
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                newnamevisible: false
                            }
                        })
                        form.resetFields();
                    }}>
                        取消
                    </Button>,
                    <Button key="submit" type="primary" onClick={handleSubmitnewname}>
                        确定
                    </Button>
                ]}
            >
                <Form onSubmit={handleSubmitnewname}>
                    <Form.Item label="名称" {...formCol}>
                        <InputGroup compact>
                            {getFieldDecorator('newname', {
                                rules: [{ required: true, message: '请输入您的新名称!' }],
                            })(
                                <Input style={{ width: '70%' }} />
                            )}
                            {fileitem.isFile ? getFieldDecorator('houzhui', {
                                // rules: [{ required: true, message: '请输入您的新名称（带后缀）!' }],
                            })(
                                <Input style={{ width: '30%' }} />
                            ) : null}
                        </InputGroup>
                    </Form.Item>
                </Form>
            </Modal>
            <Modal
                title={"标签"}
                visible={biaoqianvisible}
                onOk={handleSubmittaglist}
                onCancel={() => {
                    dispatch({
                        type: "oss/setState",
                        payload: {
                            biaoqianvisible: false
                        }
                    })
                    form.resetFields();
                }}
                footer={[
                    <Button key="back" onClick={() => {
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                biaoqianvisible: false
                            }
                        })
                        form.resetFields();
                    }}>
                        取消
                    </Button>,
                    <Button key="submit" type="primary" onClick={handleSubmittaglist}>
                        确定
                    </Button>
                ]}
            >
                <Form.Item>
                    {getFieldDecorator('taglist', {

                    })(
                        <TagSelect />
                    )}
                </Form.Item>
                <Form.Item>
                    {getFieldDecorator('applyAllChild', {

                    })(
                        <Switch />
                    )}<span style={{ margin: '5px' }}>是否运用到子目录？</span>
                </Form.Item>
            </Modal>
            <Modal
                title={"删除确认"}
                visible={deletefilevisible}
                onOk={() => {
                    if(fileitem.isFile){
                        dispatch({
                            type: "oss/DeleteObject",
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                fileName: fileitem.key
                            }
                        })
                    }else{
                        if (fileitem.name&&fileitem.name.toLowerCase() == form.getFieldValue("deletefilename").toLowerCase()) {
                        dispatch({
                            type: "oss/DeleteObject",
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                fileName: fileitem.key
                            }
                        })
                        form.resetFields()
                    } else {
                        message.info('输入名称和原文件名不符，删除失败！')
                    }
                    }
                    

                }}
                onCancel={() => {
                    dispatch({
                        type: "oss/setState",
                        payload: {
                            deletefilevisible: false
                        }
                    })
                    form.resetFields();
                }}
            >
                {!fileitem.isFile ? getFieldDecorator('deletefilename', {

                })(
                    <Input placeholder="请输入您需要删除的文件夹名称" />
                ) : "您确定要删除该文件吗？"
                }
            </Modal>
            {
                updatevisible ? <Modal
                    title={"上传文件"}
                    visible={updatevisible}
                    onCancel={() => {
                        call({
                            method: new api.AliyunOssApi().getTags,
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                key: window.sessionStorage.getItem('root'),
                            }
                        }).then(({ result }) => {
                            form.setFieldsValue({ 'tagsGroup': result, 'tagsGroup2': result })
                            createtagsurl(result)
                        });
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                updatevisible: false
                            }
                        })
                    }}
                    onOk={() => dispatch({
                        type: "oss/setState",
                        payload: {
                            updatevisible: false
                        }
                    })}
                >
                    <Row style={{ marginTop: 20 }}>
                        <Form>
                            <Form.Item label="标签组" {...formCol2}>
                                <Select style={{ width: 120 }} key="dsafsfs" onSelect={(e) => {
                                    call({
                                        method: new api.SysFunApi().getTagsByGroupId,
                                        payload: {
                                            id: e
                                        }
                                    }).then(({ result }) => {
                                        form.setFieldsValue({ 'tagsGroup': result, 'tagsGroup2': result })
                                        createtagsurl(result)
                                    });
                                }}>
                                    {
                                        dropDownList ? dropDownList.map((s, v) => (<Option value={s.value} key={s.name}>{s.name}</Option>)) : null
                                    }
                                </Select>
                            </Form.Item>
                            <Form.Item>
                                {getFieldDecorator('tagsGroup', {
                                    // initialValue: window.sessionStorage.getItem('tagNames')?window.sessionStorage.getItem('tagNames').split(',').splice(window.sessionStorage.getItem('tagNames').split(',').length-1):null
                                })(
                                    <TagSelect onChange={(e) => createtagsurl(e)} />
                                )}
                            </Form.Item>
                        </Form>
                        <Col span={24}>
                            <Dragger {...props}>
                                <p className="ant-upload-drag-icon">
                                    <Icon type="inbox" />
                                </p>
                                <p className="ant-upload-text">拖拽文件到此处</p>
                                <p className="ant-upload-hint">
                                    支持ctrl多选拖入上传或者点击选择文件上传
                        </p>
                            </Dragger>
                        </Col>
                    </Row>
                </Modal> : null
            }
            {
                wenjianjiavisible ? <Modal
                    title={"上传文件夹"}
                    visible={wenjianjiavisible}
                    onCancel={() => {
                        call({
                            method: new api.AliyunOssApi().getTags,
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                key: window.sessionStorage.getItem('root'),
                            }
                        }).then(({ result }) => {
                            form.setFieldsValue({ 'tagsGroup': result })
                            createtagsurl(result)
                        });
                        dispatch({
                            type: "oss/setState",
                            payload: {
                                wenjianjiavisible: false
                            }
                        })
                    }}
                    onOk={() => dispatch({
                        type: "oss/setState",
                        payload: {
                            wenjianjiavisible: false
                        }
                    })}
                >
                    <Row style={{ marginTop: 20 }}>
                        <Form>
                            <Form.Item label="标签组" {...formCol2}>
                                <Select style={{ width: 120 }} key="dsafsfs" onSelect={(e) => {
                                    call({
                                        method: new api.SysFunApi().getTagsByGroupId,
                                        payload: {
                                            id: e
                                        }
                                    }).then(({ result }) => {
                                        form.setFieldsValue({ 'tagsGroup': result, 'tagsGroup2': result })
                                        createtagsurl(result)
                                    });
                                }}>
                                    {
                                        dropDownList ? dropDownList.map((s, v) => (<Option value={s.value} key={s.name}>{s.name}</Option>)) : null
                                    }
                                </Select>
                            </Form.Item>
                            <Form.Item>
                                {getFieldDecorator('tagsGroup2', {
                                    // initialValue: window.sessionStorage.getItem('tagNames') ? window.sessionStorage.getItem('tagNames').split(',') : null
                                })(
                                    <TagSelect onChange={(e) => createtagsurl(e)} />
                                )}
                            </Form.Item>
                        </Form>
                        <Col span={24}>
                            <Dragger {...props2} directory onClick={null}>
                                <p className="ant-upload-drag-icon">
                                    <Icon type="inbox" />
                                </p>
                                <p className="ant-upload-text">拖拽文件夹到此处</p>
                                <p className="ant-upload-hint">
                                    支持ctrl多选拖入上传或者点击选择文件上传
                        </p>
                            </Dragger>
                        </Col>
                    </Row>
                </Modal> : null
            }

        </div>
    );

}

File = connect((state) => {
    return {
        ...state.oss
    };
})(File);
export default create()(File);
