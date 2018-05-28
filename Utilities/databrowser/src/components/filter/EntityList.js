import React from 'react';
import { render } from 'react-dom';
import Divider from '@material-ui/core/Divider';
import Paper from '@material-ui/core/Paper';
import ListSubheader from '@material-ui/core/ListSubheader';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import Typography from '@material-ui/core/Typography';
import TextField from '@material-ui/core/TextField';
import FormControl from '@material-ui/core/FormControl';
import InputAdornment from '@material-ui/core/InputAdornment';
import Search from '@material-ui/icons/Search';
import Chronicity from '../../clients/Chronicity.js'

class EntityList extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      searchText: '',
      results: []
    };
  }

  handleChange = prop => event => {
    this.setState({ [prop]: event.target.value });

    var that = this;

    Chronicity.searchEntities(event.target.value)
      .then((res) => { return res.json(); })
      .then((result) => {
          that.setState({
            results: result
          });
        }
      );
  };

  render() {
    const { classes } = this.props;
    const { value } = this.state;

    return <div className={classes.root}>
          <Paper className={classes.paper}>
            <FormControl className={classes.margin}>
              <TextField
                id="EntitySearchText"
                label="Search Entities"
                className={classes.textField}
                value={this.state.searchText}
                margin="none"
                onChange={this.handleChange('searchText')}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <Search />
                    </InputAdornment>
                  ),
                }}
              />
            </FormControl>
            <List component="nav">
              {this.state.results.map(n => {
                return (
                  <ListItem key={n} >
                    <ListItemText primary={n} />
                  </ListItem>
                );
              })}
           </List>
          </Paper>
        </div>
  }
}

export default (EntityList);
