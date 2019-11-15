/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow
 */

import React from 'react';
import {
    StyleSheet,
    View,
    Text,
} from 'react-native';
// import { Button,Badge } from '@ant-design/react-native';
import { Actions, Router, Scene } from 'react-native-router-flux';
import { Button } from 'teaset';
const Home: () => React$Node = () => {
    return (
        <>
            <Button type="primary">
                <Text onPress={() => Actions.index()}>
                    我是home
            </Text>
            </Button>
        </>
    );
};

const styles = StyleSheet.create({

});

export default Home;
