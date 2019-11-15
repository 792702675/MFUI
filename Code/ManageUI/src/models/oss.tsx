import React from 'react';
import { notification } from "antd";
import * as api from './../api/api';
import { createApiAuthParam } from './../api/apiUtil';

export default {
    namespace: 'oss',
    state: {
        buckets: [],
        visible: false,
        can:false,
        bucketName: '',
        resultall: [],
        visiblefolder: false,
        imgvisible: false,
        item: '',
        leftbox: 0,
        topbox: 0,
        visiblefixed: false,
        checkedList: [],
        copyvisible: false,
        treeData: [],
        treelist: null,
        pdfvisible: false,
        url: '',
        fileitem: '',
        rootstrees: '',
        gogobuckets: '',
        modaltype: "",
        checkarrey: [],
        pi: false,
        isLeaf: false,
        biaoqianvisible: false,
        tags: ['Tag 2', 'Tag 3'],
        inputVisible: false,
        inputvalue: '',
        addtag: '',
        searchvalue: '',
        mingcheng: true,
        leixintg: true,
        shijian: true,
        chicun: true,
        checklist: [],
        issearch: false,
        spinningloading: false,
        videovisible: false,
        checkedbox:false,
        dropDownList:[],
        getTagsByGroupIds:[],
        startindex:0,
        updatevisible:false,
        wenjianjiavisible:false,
        deletefilevisible:false,
        deletebucketvisible:false,
        buckitem:'',
        tagOptionsprops:''
    },
    reducers: {
        setState(state, { payload }) {
            return {
                ...state,
                ...payload
            };
        }
    },
    effects: {
        *GetBuckets({ payload }, { call, put }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().getBuckets,
                payload: payload
            }));
            if (success) {
                yield put({
                    type: "setState",
                    payload: {
                        buckets: result
                    }
                })
                yield put({
                    type: "cancreate"
                })
                let buckets = [];
                for (var i = 0; i < result.length; i++) {
                    buckets.push(result[i].name)
                }
                window.sessionStorage.setItem('buckets', buckets)
            }
        },
        *CreateBucket({ payload }, { call, put }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().createBucket,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "添加成功!",
                    description: <span>您已成功添加bucket</span>
                });
                yield put({
                    type: "GetBuckets"
                })
                yield put({
                    type: "setState",
                    payload: {
                        visible: false
                    }
                })
            }
        },
        *DeleteBucket({ payload }, { call, put }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().deleteBucket,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "删除成功!",
                    description: <span>您已成功删除存储库成功</span>
                });
                yield put({
                    type: "GetBuckets"
                })
                yield put({
                    type: "cancreate"
                })
                yield put({
                    type:'setState',
                    payload:{
                        deletebucketvisible:false
                    }
                })
            }
        },
        *rename({ payload }, { call, put }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().rename,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "修改成功!",
                    description: <span>您已成功修改名称</span>
                });
                yield put({
                    type:"setState",
                    payload:{
                        newnamevisible:true
                    }
                })
                yield put({
                    type: 'getObjectListOfDirectory',
                    payload: {
                        bucketName: window.sessionStorage.getItem('bucketName'),
                        root: window.sessionStorage.getItem('root')
                    }
                })
            }
        },
        *getObjectListOfDirectory({ payload, treeNode, callback }, { call, put, select }) {
            var treeData = yield select(({ oss }) => oss.treeData);
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().getObjectListOfDirectory,
                payload: payload
            }));
            if (success) {
                if (payload.directoryOnly) {
                    treeNode.props.dataRef.children = [];
                    result ? result.map((item, index) => {
                        treeNode.props.dataRef.children.push({ title: item.name, key: `${treeNode.props.eventKey}-${item.name}` })
                    }) : null;
                    yield put({
                        type: "setState",
                        payload: {
                            treeData: [...treeData]
                        }
                    })
                    callback();
                } else {
                    yield put({
                        type: "setState",
                        payload: {
                            resultall: result,
                        }
                    })
                }



            }
        },
        *CreateFolder({ payload }, { call, put }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().createFolder,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "创建成功!",
                    description: <span>您已成功创建文件夹，您可以向里添加文件啦！！</span>
                });
                yield put({
                    type: 'getObjectListOfDirectory',
                    payload: {
                        bucketName: window.sessionStorage.getItem('bucketName'),
                        root: window.sessionStorage.getItem('root')
                    }
                })
                yield put({
                    type: 'setState',
                    payload: {
                        visiblefolder: false
                    }
                })
            }
        },
        *DeleteObject({ payload }, { call, put, select }) {
            var state = yield select(({ oss }) => oss);
            console.log(state)
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().deleteObject,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "删除成功!",
                    description: <span>您已成功删除</span>
                });
                if (state.issearch) {
                    yield put({
                        type: 'search',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root'),
                            ...state.searchvalue
                        }
                    })
                } else {
                    yield put({
                        type: 'getObjectListOfDirectory',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root')
                        }
                    })
                }
                yield put({
                    type: "setState",
                    payload: {
                        deletefilevisible: false
                    }
                })

            }
        },
        *DeleteObjects({ payload }, { call, put, select }) {
            var state = yield select(({ oss }) => oss);
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().deleteObjects,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: result ? "删除成功!" : "操作成功",
                    description: <span>{result ? '您已成功删除' : '操作成功，但非空目录不允许直接删除！'}</span>
                });
                if (state.issearch) {
                    yield put({
                        type: 'search',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root'),
                            ...state.searchvalue
                        }
                    })
                } else {
                    yield put({
                        type: 'getObjectListOfDirectory',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root')
                        }
                    })
                }
            }
        },
        *copy({ payload }, { call, put, select }) {
            var state = yield select(({ oss }) => oss);
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().copy,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "复制成功!",
                    description: <span>复制成功</span>
                });
                if (state.issearch) {
                    yield put({
                        type: 'search',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root'),
                            ...state.searchvalue
                        }
                    })
                } else {
                    yield put({
                        type: "getObjectListOfDirectory",
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root')
                        }
                    })
                }
                yield put({
                    type: "setState",
                    payload: {
                        copyvisible: false,
                        treeData: []
                    }
                })
            }
        },
        *copyBatch({ payload }, { call, put, select }) {
            var state = yield select(({ oss }) => oss);
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().copyBatch,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "复制成功!",
                    description: <span>复制成功</span>
                });
                if (state.issearch) {
                    yield put({
                        type: 'search',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root'),
                            ...state.searchvalue
                        }
                    })
                } else {
                    yield put({
                        type: "getObjectListOfDirectory",
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root')
                        }
                    })
                }
                yield put({
                    type: "setState",
                    payload: {
                        copyvisible: false,
                        treeData: []
                    }
                })
            }
        },
        *moveBatch({ payload }, { call, put, select }) {
            var state = yield select(({ oss }) => oss);
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().moveBatch,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "移动成功!",
                    description: <span>移动成功</span>
                });
                if (state.issearch) {
                    yield put({
                        type: 'search',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root'),
                            ...state.searchvalue
                        }
                    })
                } else {
                    yield put({
                        type: "getObjectListOfDirectory",
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root')
                        }
                    })
                }
                yield put({
                    type: "setState",
                    payload: {
                        copyvisible: false,
                        treeData: []
                    }
                })
            }
        },
        *move({ payload }, { call, put, select }) {
            var state = yield select(({ oss }) => oss);
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().move,
                payload: payload
            }));
            if (success) {
                if (payload.sub) {
                    notification.success({
                        message: "命名成功!",
                        description: <span>你重命名成功</span>
                    });
                    yield put({
                        type: 'setState',
                        payload: {
                            newnamevisible: false
                        }
                    })
                    if (state.issearch) {
                        yield put({
                            type: 'search',
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                root: window.sessionStorage.getItem('root'),
                                ...state.searchvalue
                            }
                        })
                    } else {
                        yield put({
                            type: "getObjectListOfDirectory",
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                root: window.sessionStorage.getItem('root')
                            }
                        })
                    }
                } else {
                    notification.success({
                        message: "移动成功!",
                        description: <span>移动成功</span>
                    });
                    if (state.issearch) {
                        yield put({
                            type: 'search',
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                root: window.sessionStorage.getItem('root'),
                                ...state.searchvalue
                            }
                        })
                    } else {
                        yield put({
                            type: "getObjectListOfDirectory",
                            payload: {
                                bucketName: window.sessionStorage.getItem('bucketName'),
                                root: window.sessionStorage.getItem('root')
                            }
                        })
                    }
                    yield put({
                        type: "setState",
                        payload: {
                            copyvisible: false,
                            treeData: []
                        }
                    })
                }

            }
        },
        *updateTag({ payload }, { call, put ,select}) {
            var state = yield select(({ oss }) => oss);
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().updateTag,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "更新标签成功!",
                    description: <span>更新标签成功</span>
                });
                if (state.issearch) {
                    yield put({
                        type: 'search',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root'),
                            ...state.searchvalue
                        }
                    })
                } else {
                    yield put({
                        type: "getObjectListOfDirectory",
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root')
                        }
                    })
                }
                yield put({
                    type: "setState",
                    payload: {
                        biaoqianvisible: false
                    }
                })
            }
        },
        *batchAddTag({ payload }, { call, put,select }) {
            var state = yield select(({ oss }) => oss);
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().batchAddTag,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "添加标签成功!",
                    description: <span>批量添加标签成功</span>
                });
                if (state.issearch) {
                    yield put({
                        type: 'search',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root'),
                            ...state.searchvalue
                        }
                    })
                } else {
                    yield put({
                        type: "getObjectListOfDirectory",
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root')
                        }
                    })
                }
                yield put({
                    type: "setState",
                    payload: {
                        biaoqianvisible: false
                    }
                })
            }
        },
        *batchSubTag({ payload }, { call, put, select }) {
            var state = yield select(({ oss }) => oss);
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().batchSubTag,
                payload: payload
            }));
            if (success) {
                notification.success({
                    message: "删除标签成功!",
                    description: <span>批量删除标签成功</span>
                });
                if (state.issearch) {
                    yield put({
                        type: 'search',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root'),
                            ...state.searchvalue
                        }
                    })
                } else {
                    yield put({
                        type: "getObjectListOfDirectory",
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root')
                        }
                    })
                }
                yield put({
                    type: "setState",
                    payload: {
                        biaoqianvisible: false
                    }
                })
            }
        },
        *search({ payload }, { call, put }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().search,
                payload: payload
            }));
            if (success) {
                yield put({
                    type: "setState",
                    payload: {
                        resultall: result,
                    }
                })
            }
        },
        *cancreate({ payload }, { call, put }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.AliyunOssApi().canCreateAsync,
                payload: payload
            }));
            if (success) {
                yield put({
                    type: "setState",
                    payload: {
                        can: result,
                    }
                })
            }
        },
        *getDropDownList({ payload }, { call, put }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.SysFunApi().getDropDownList,
            }));
            if (success) {
                yield put({
                    type: "setState",
                    payload: {
                        dropDownList: result,
                    }
                })
            }
        },
        *getTagsByGroupId({ payload }, { call, put }) {
            const { success, result } = yield call(...createApiAuthParam({
                method: new api.SysFunApi().getTagsByGroupId,
                payload:payload
            }));
            if (success) {
                yield put({
                    type: "setState",
                    payload: {
                        getTagsByGroupIds: result,
                    }
                })
            }
        },
    },
    subscriptions: {
        setup({ dispatch, history }) {
            return history.listen(({ pathname, state }) => {
                if (pathname.toLowerCase() == '/home/resources'.toLowerCase()) {
                    dispatch({
                        type: 'GetBuckets'
                    });
                    dispatch({
                        type:"cancreate"
                    })
                }
                if (pathname.toLowerCase() == '/home/resources/file'.toLowerCase()) {
                    // window.sessionStorage.getItem('tagurl') ? form.setFieldsValue({ 'tagsGroup': window.sessionStorage.getItem('tagurl') }) : null
                    dispatch({
                        type: 'getObjectListOfDirectory',
                        payload: {
                            bucketName: window.sessionStorage.getItem('bucketName'),
                            root: window.sessionStorage.getItem('root')
                        }
                    });
                    dispatch({
                        type: "getDropDownList"
                    })
                }
            });
        },
    },
};
