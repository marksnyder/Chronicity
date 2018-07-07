import React from 'react';
import PropTypes from 'prop-types';
import TimelineView from './components/timeline/View.js'
import MenuView from './components/menu/View.js'
import CodeView from './components/codeedit/View.js'
import { MuiThemeProvider, createMuiTheme,withStyles } from '@material-ui/core/styles';
import CodeRunner from './helpers/CodeRunner.js'
import Chronicity from './helpers/Chronicity.js'
import DataUtilities from './helpers/DataUtilities.js'

const theme = createMuiTheme({
  palette: {
    primary: {
      // light: will be calculated from palette.primary.main,
      main: '#2e3336'
      // dark: will be calculated from palette.primary.main,
      // contrastText: will be calculated to contast with palette.primary.main
    },
    secondary: {
      main: '#3498db'
    },
    // error: will use the default color
  },
});

const styles = theme => ({
  root: {
    flexGrow: 1,
    backgroundColor: theme.palette.background.paper
  },
  flex: {
      flex: 1,
  },
  margin: {
    margin: theme.spacing.unit
  }
});


class App extends React.Component {


  constructor(props) {
    super(props);
    this.runner = new CodeRunner();
    this.runner.loadCode();
    this.state = {
      view: 'code',
      events: [],
      stateChanges: []
    };
  }

  changeView = (view) => {
    this.setState({
      view: view
    });
  };

  setStateChanges = (data) => {
    this.setState({
      stateChanges: data
    });
  };

  setEvents = (data) => {
    this.setState({
      events: data
    });
  };

  getClient = () => {
    return Chronicity;
  };

  getUtilities = () => {
    return DataUtilities;
  }

  runCode = () => {
      this.runner.runCode(this);
  }

  saveCode = () => {
      this.runner.saveCode();
  }

  setCode = (code) => {
      this.runner.setCode(code);
  }

  getCode = () => {
    return this.runner.getCode();
  }

  render() {
    const { classes } = this.props;
    const initialCode = this.getCode();

    return (
    <MuiThemeProvider theme={theme}>
      <MenuView classes={classes} changeView={this.changeView} />
      {this.state.view == 'timeline' &&
        <TimelineView
          classes={classes}
          events={this.state.events}
          stateChanges={this.state.stateChanges}
        />
      }
      {this.state.view == 'code' &&
        <CodeView classes={classes}
          addEvents={this.addEvents}
          clearEvents={this.clearEvents}
          addStateChanges={this.addStateChanges}
          clearStateChanges={this.clearStateChanges}
          initialCode={initialCode}
          runCode={this.runCode}
          saveCode={this.saveCode}
          setCode={this.setCode}
         />
      }
    </MuiThemeProvider>
    );
  }
}


export default withStyles(styles)(App);
