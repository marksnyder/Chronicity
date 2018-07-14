import React from 'react';
import TimelineView from './components/timeline/View.js'
import MenuView from './components/menu/View.js'
import { MuiThemeProvider, createMuiTheme,withStyles } from '@material-ui/core/styles';
import BirdActivity from './examples/BirdActivity.js'

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
    this.state = {
      events: [],
      stateChanges: []
    };
  }

  changeView = (view) => {
    this.setState({
      view: view
    });
  };

  setStream = (events,stateChanges) => {
    this.setState({
      stateChanges: stateChanges,
      events: events
    });
  };

  componentDidMount = () => {
    var ex = new BirdActivity();
    ex.runExample(this);
  }


  render() {

    return (
    <MuiThemeProvider theme={theme}>
      <MenuView changeView={this.changeView} />
      <TimelineView
        events={this.state.events}
        stateChanges={this.state.stateChanges}
      />
    </MuiThemeProvider>
    );
  }
}


export default withStyles(styles)(App);
