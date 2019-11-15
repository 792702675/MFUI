import React from 'react';
import echarts from 'echarts';
class SkillChart extends React.Component {
	componentDidMount() {
		let _this = this;
		let myChart = echarts.init(_this.refs.chart);
		let size = 50;
		let listdata = [];
		let links = [];
		let legendes = [];
		let i = -1;
		const loop = (data) =>
			data.map((item) => {
				i++;
				legendes[i] = item.name;
				listdata.push({
					x: i * 50,
					y: size + i * 10,
					name: item.name,
					symbolSize: size,
					category: i,
					draggable: true
				});
				if (item.subordinateSkills && item.subordinateSkills.length) {
					loop(item.subordinateSkills);
					item.subordinateSkills.forEach((x) => {
						links.push({
							source: x.name,
							target: item.name,
							lineStyle: {
								normal: {
									color: 'source'
								}
							}
						});
					});
				}
			});

		loop(this.props.data);
		listdata[0].x = (listdata[listdata.length - 1].x + listdata[0].x + size) / 2;
		listdata[0].y = (listdata[listdata.length - 1].y + listdata[0].y + size) / 2;
		listdata[0].draggable = false;
		listdata[0].fixed = true;

		myChart.setOption({
			title: {
				text: '技能树',
				top: 'top',
				left: 'left',
				textStyle: {
					color: '#f7f7f7'
				}
			},
			tooltip: {
				formatter: '{b}'
			},
			toolbox: {
				show: true,
				feature: {
					restore: {
						show: true
					},
					saveAsImage: {
						show: true
					}
				}
			},
			backgroundColor: '#00000',
			// legend: {
			// 	data: legendes,
			// 	textStyle: {
			// 		color: '#fff'
			// 	},
			// 	icon: 'circle',
			// 	type: 'scroll',
			// 	orient: 'vertical',
			// 	left: 10,
			// 	top: 20,
			// 	bottom: 20,
			// 	itemWidth: 10,
			// 	itemHeight: 10
			// },
			animationDuration: 1000,
			animationEasingUpdate: 'quinticInOut',
			series: [
				{
					name: '技能图谱',
					type: 'graph',
					layout: 'force',
					force: {
						repulsion: 500,
						gravity: 0.1,
						edgeLength: 100,
						layoutAnimation: true
					},
					data: listdata,
					links: links,
					categories: legendes.map((x) => {
						return { name: x };
					}),
					roam: false,
					label: {
						normal: {
							show: true,
							position: 'inside',
							formatter: '{b}',
							fontSize: 16,
							fontStyle: '600'
						}
					},
					lineStyle: {
						normal: {
							opacity: 0.9,
							width: 1.5,
							curveness: 0
						}
					}
				}
			]
		});
	}

	render() {
		return <div ref="chart" style={{ height: 600 }} />;
	}
}

export default SkillChart;
