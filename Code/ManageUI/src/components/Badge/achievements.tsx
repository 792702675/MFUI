import React from 'react';
import { connect } from 'dva';
import { Form, Card, Button, Icon, Select, Input, Col, Row, Popover } from 'antd';
const create = Form.create;
const FormItem = Form.Item;
const InputGroup = Input.Group;
import * as api from '../../api/api';
import EnumSelect from '@/components/System/EnumSelect';
import { call } from '../../api/apiUtil';
import SkillTreeSelect from '@/components/System/SkillTreeSelect';
import QuestTreeSelect from '@/components/System/QuestTreeSelect';
export default class CollectionItemInput extends React.Component {
    constructor() {
        super();
        this.state = {
            result: [],
            place: '',
            newsub: true,
            worldlist: [],
            worldname: '选择',
            skilid:[],
            questid:[],
            wanquestid:[]
            // ossdas:''
        };
    }
    componentDidMount() {
        call({
            method: this.props.api,
        }).then(({ result }) => {
            this.setState({ result: result });
        });
    }
    huode = e => {
        for (var i = 0; i < this.state.result.length; i++) {
            if (e == this.state.result[i].name) {
                var exampleValue =
                    // this.state.result[i].conditionsRegex ? this.state.result[i].conditionsRegex.match(/“(.+)”/) ? 
                    this.state.result[i]
                // : '无示例' :'无示例';
                // this.setState({
                //     place: exampleValue,
                //     // ossdas: this.state.result[i].condition
                // })
                return exampleValue
            }
        }
    }
    render() {
        const { value = [], onChange, type } = this.props;
        return (
            <div>
                <div>

                    {value ? value.map((a, i) => (
                        <InputGroup key={i} style={{ marginBottom: '8px' }}>
                            <Row gutter={10}>
                                <Col span={10}>
                                    <EnumSelect
                                        value={a.conditionType}
                                        onChange={(e) => {
                                            console.log(e)
                                            if (e == "微型世界完成进度") {
                                                call({
                                                    method: new api.MicroworldApi().getAll,
                                                }).then(({ result }) => {
                                                    this.setState({ worldlist: result.items });
                                                });
                                            } else if (e == "区域完成进度") {
                                                call({
                                                    method: new api.AreaApi().getAll,
                                                }).then(({ result }) => {
                                                    this.setState({ worldlist: result.items });
                                                });
                                            } 
                                            // else if (e == "技能等级") {
                                            // } else if (e == "谜题通过" || e == "谜题完美通过") {

                                            // } else if (e == "是否加入班组") {

                                            // }
                                            let newMarkers = [...value];
                                            newMarkers[i].conditionType = e;
                                            newMarkers[i].condition = this.huode(e).example;
                                            newMarkers[i].conditionsub = this.huode(e).note;
                                            newMarkers[i].conditionsRegex = this.huode(e).conditionsRegex;
                                            newMarkers[i].tishi = true;
                                            newMarkers[i].disabled = true;
                                            console.log(newMarkers)
                                            onChange(newMarkers);
                                        }}
                                        api={this.props.api}
                                        payload={{ type: type }}
                                        optionRender={(x) => (
                                            <Select.Option value={x.name}>
                                                {x.name}
                                            </Select.Option>
                                        )}
                                    />
                                </Col>
                                <Col span={12}>
                                    {
                                        value[i].conditionType == "微型世界完成进度" || value[i].conditionType == "区域完成进度" ?
                                            <Col span={24}>
                                                <Col span={12}>
                                                    <Popover trigger="click" content={<div>{this.state.worldlist ? this.state.worldlist.map((a, b) => (
                                                        <div style={{ marginBottom: 10, cursor: 'pointer' }} onClick={() => {
                                                            let newMarkers = [...value];
                                                            let condition = JSON.parse(newMarkers[i].condition);
                                                            condition.MicroworldId = a.id;
                                                            condition.name = a.name;
                                                            console.log(condition)
                                                            newMarkers[i].condition = JSON.stringify(condition);
                                                            onChange(newMarkers);
                                                            this.setState({
                                                                worldname: a.name
                                                            })
                                                        }}>
                                                            <img src={value[i].conditionType == "微型世界完成进度" ? a.backgroundImage.image.url : a.areaImage.image.url} style={{ maxWidth: '100px', maxHeight: '100px' }} alt="" />
                                                            <span>{a.name}</span>
                                                        </div>
                                                    )) : null}</div>} title="选择">
                                                        <Button onClick={() => {
                                                            if (value[i].conditionType == "微型世界完成进度") {
                                                                call({
                                                                    method: new api.MicroworldApi().getAll,
                                                                }).then(({ result }) => {
                                                                    this.setState({ worldlist: result.items });
                                                                });
                                                            } else if (value[i].conditionType == "区域完成进度") {
                                                                call({
                                                                    method: new api.AreaApi().getAll,
                                                                }).then(({ result }) => {
                                                                    this.setState({ worldlist: result.items });
                                                                });
                                                            }
                                                        }}>{JSON.parse(value[i].condition).name ? JSON.parse(value[i].condition).name : this.state.worldname}</Button>
                                                    </Popover>
                                                </Col>
                                                <Col span={12}>
                                                    <Input
                                                        value={JSON.parse(a.condition).Progress}
                                                        placeholder="进度"
                                                        onChange={(e) => {
                                                            let newMarkers = [...value];
                                                            let condition = JSON.parse(newMarkers[i].condition);
                                                            condition.Progress = e.target.value;
                                                            newMarkers[i].condition = JSON.stringify(condition);
                                                            console.log(newMarkers[i])
                                                            onChange(newMarkers);
                                                        }}
                                                    />
                                                </Col>
                                            </Col> :
                                            value[i].conditionType == "是否加入班组" ?
                                                null
                                                :
                                            value[i].conditionType == "技能等级" ?
                                                    <Col span={24}>
                                                        <Col span={12}>
                                                            <SkillTreeSelect value={this.state.skilid.length !== 0 ? this.state.skilid : JSON.parse(a.condition).SkillId} treeCheckable={true} multiple={true} onChange={(e)=>{
                                                                this.setState({
                                                                    skilid:e
                                                                })
                                                                let newMarkers = [...value];
                                                                let condition = JSON.parse(newMarkers[i].condition);
                                                                condition.SkillId = e;
                                                                console.log(condition)
                                                                newMarkers[i].condition = JSON.stringify(condition);
                                                                onChange(newMarkers);
                                                            }}/>
                                                        </Col>
                                                        <Col span={12}>
                                                            <Input
                                                                value={JSON.parse(a.condition).Level}
                                                                onChange={(e) => {
                                                                    let newMarkers = [...value];
                                                                    let condition = JSON.parse(newMarkers[i].condition);
                                                                    condition.Level = e.target.value;
                                                                    newMarkers[i].condition = JSON.stringify(condition);
                                                                    console.log(newMarkers[i])
                                                                    onChange(newMarkers);
                                                                }}
                                                            />
                                                        </Col>
                                                    </Col>
                                                    :
                                                value[i].conditionType == "谜题通过" ?
                                                        <Col span={24}>
                                                            <QuestTreeSelect value={this.state.questid.length !== 0 ? this.state.questid : a.condition} treeCheckable={true} multiple={true}  onChange={(e)=>{
                                                                this.setState({
                                                                    questid:e
                                                                })
                                                                let newMarkers = [...value];
                                                                newMarkers[i].condition = e;
                                                                console.log(newMarkers[i])
                                                                onChange(newMarkers);
                                                            }}/>
                                                        </Col>       
                                                    :
                                                    value[i].conditionType == "谜题完美通过" ?
                                                            <Col span={24}>
                                                                <QuestTreeSelect value={this.state.wanquestid.length !== 0 ? this.state.wanquestid : a.condition} treeCheckable={true} multiple={true} onChange={(e) => {
                                                                    this.setState({
                                                                        wanquestid: e
                                                                    })
                                                                    let newMarkers = [...value];
                                                                    newMarkers[i].condition = e;
                                                                    console.log(newMarkers[i])
                                                                    onChange(newMarkers);
                                                                }} />
                                                            </Col> 
                                                            :
                                                    <Col span={24}>
                                                        <Input
                                                            value={a.condition}
                                                            onChange={(e) => {
                                                                let newMarkers = [...value];
                                                                newMarkers[i].condition = e.target.value;
                                                                newMarkers[i].tishi = new RegExp(a.conditionsRegex).test(e.target.value);
                                                                onChange(newMarkers);
                                                            }}
                                                        />
                                                    </Col>
                                    }
                                </Col>
                                <Col span={2}>
                                    <Icon
                                        type="delete"
                                        onClick={() => {
                                            let newMarkers = [...value];
                                            newMarkers.splice(i, 1);
                                            onChange(newMarkers);
                                        }}
                                    />
                                </Col></Row>
                            {/* <Row>{a.tishi ? a.conditionsub : <div style={{ color: 'red' }}>{'请根据所选达成条件，参考所选的类型的格式提示'}</div>}</Row> */}
                        </InputGroup>

                    )) : null}
                    <center>
                        <Button
                            onClick={() => {
                                let newValue = [...value, {}];
                                onChange(newValue);
                            }}
                        >
                            添加
					</Button>
                    </center>
                </div>
            </div>
        );
    }
}
