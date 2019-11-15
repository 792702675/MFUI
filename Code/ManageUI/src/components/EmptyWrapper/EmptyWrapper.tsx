import React, { Component } from 'react';
import { connect } from 'dva';
class EmptyWrapper extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        if (this.props.children && this.props.children.type && this.props.children.type.name === "Menu") {
            var children = [];
            for (var i in this.props.children.props.children) {
                if (!this.props.children.props.children[i] || !this.props.children.props.children[i].props) {
                    continue;
                }
                children.push(this.props.children.props.children[i]);
            }
            return React.cloneElement(this.props.children, { children: children });
        }
        return this.props.children;
    }
}

export default connect((state) => {
    return {}
})(EmptyWrapper);
