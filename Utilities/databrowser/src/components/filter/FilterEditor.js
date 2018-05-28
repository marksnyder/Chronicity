import React from 'react';
import { render } from 'react-dom';
import brace from 'brace';
import AceEditor from 'react-ace';
import 'brace/mode/json';
import 'brace/theme/github';
import Divider from '@material-ui/core/Divider';
import Paper from '@material-ui/core/Paper';
import Grid from '@material-ui/core/Grid';
import AppBar from '@material-ui/core/AppBar';
import Button from '@material-ui/core/Button';
import Toolbar from '@material-ui/core/Toolbar';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';

class FilterEditor extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      value: 0,
      filterText: JSON.stringify(this.props.filters, null, 2)
    };
  }

  changeFilterText = (filterText) => {
    this.setState({
      filterText: filterText
    });
  }

  runFilters = () => {
    var filters = JSON.parse(this.state.filterText);
    this.props.applyFilters(filters);
  }

  render() {
    const { classes } = this.props;
    const { value } = this.state;

    return  <div className={classes.root}>
    <AppBar color="secondary" position="static">
        <Toolbar>
          <Typography variant="subheading" color="inherit" className={classes.flex}>
          Filter Query
        </Typography>
          <IconButton size="small" aria-label="Play/pause">
            <PlayArrowIcon onClick={this.runFilters} className={classes.playIcon} />
          </IconButton>
        </Toolbar>
      </AppBar>
      <Paper className={classes.paper}>
        <AceEditor
          mode="json"
          theme="github"
          name="filterEditor"
          fontSize={14}
          showPrintMargin={true}
          showGutter={true}
          highlightActiveLine={true}
          value={this.state.filterText}
          onChange={this.changeFilterText}
        />
      </Paper>
    </div>
  }
}

export default (FilterEditor);
