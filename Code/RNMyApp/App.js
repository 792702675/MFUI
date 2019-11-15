/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow
 */

import React from 'react';
import Index from './components/Index';
import Home from './components/Home';
import Audio from './components/Audio';
import File from './components/File';
import {
  SafeAreaView,
  StyleSheet,
  ScrollView,
  View,
  Text,
  StatusBar,
  TouchableOpacity
} from 'react-native';
import { Actions, Router, Scene } from 'react-native-router-flux';
import { Button } from 'teaset';
import Http from "./url/feach";
// import SplashScreen from 'react-native-splash-screen'
export class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      visiblekey: ''
    }
  }
  // componentDidMount(){
  //      setTimeout(()=>{SplashScreen.hide()}, 3000, )
  //    };
  render() {



    return (
      <Router>
        <Scene>
          <Scene key="index" component={Index} title="index" initial="true" />
          {/* <Scene key="home" component={Home} title="home"  /> */}
          <Scene key="audio" component={Audio} title="audio"  />
          <Scene key="file" component={File} title="file" hideNavBar='true' />
        </Scene>
      </Router>
    );
  }
};

const styles = StyleSheet.create({
  rightbarbox: {
    width: 50,
    height: 50,
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center'
  },
  rightbottom: {
    fontSize: 28,
    color: '#000'
  }
});

export default App;
