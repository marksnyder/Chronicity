import DataUtilities from './DataUtilities.js';
import moment from 'moment';

/********************** MergeStateChanges ******************/

it('MergeStateChanges Updates Group', () => {

    var source = [];
    source['test'] = [{ key: 'more stuff' }];

    var changes = [{ key: 'stuff'}];

    var result = DataUtilities.mergeStateChanges(source,changes, 'test', '');

    expect(result['test'].length).toEqual(2);

});

it('MergeStateChanges Creates Group', () => {

    var source = [];

    var changes = [{ key: 'stuff'}];

    var result = DataUtilities.mergeStateChanges(source,changes, 'test', '');

    expect(result['test'].length).toEqual(1);

});

it('MergeStateChanges Seperates Groups', () => {

    var source = [];
    source['test'] = [{ key: 'more stuff' }];

    var changes = [{ key: 'stuff'}];

    var result = DataUtilities.mergeStateChanges(source,changes, 'test2', '');

    expect(result['test'].length).toEqual(1);
    expect(result['test2'].length).toEqual(1);
});

it('MergeStateChanges Applies Color', () => {

    var source = [];

    var changes = [{ key: 'stuff'}];

    var result = DataUtilities.mergeStateChanges(source,changes, 'test', 'red');

    expect(result['test'][0].color).toEqual('red');

});

/********************** MergeEvents ******************/


it('MergeEvents Adds Events', () => {

    var source = [];

    var changes = [{ key: 'stuff'}];

    var result = DataUtilities.mergeEvents(source,changes,'icon','initials');

    expect(result.length).toEqual(1);

});


it('MergeEvents Merges Events', () => {

    var source = [{ key: 'more stuff'}];

    var changes = [{ key: 'stuff'}];

    var result = DataUtilities.mergeEvents(source,changes,'icon','initials');

    expect(result.length).toEqual(2);

});


/********************** FindApplicableStateChanges ******************/

it('FindApplicableStateChanges Includes Direct Match', () => {

    var source = [{ start: '2018-06-01' , end: '2018-06-02'}];

    var result = DataUtilities.findApplicableStateChanges('2018-05-01', '2018-07-01', source);

    expect(result.length).toEqual(1);

});

it('FindApplicableStateChanges Includes Overlap 1 Match', () => {

    var source = [{ start: '2018-04-01', end: '2018-06-01'}];

    var result = DataUtilities.findApplicableStateChanges('2018-05-01', '2018-07-01', source);

    expect(result.length).toEqual(1);

});

it('FindApplicableStateChanges Includes Overlap 2 Match', () => {

    var source = [{ start: '2018-06-01' , end: '2018-08-01'}];

    var result = DataUtilities.findApplicableStateChanges('2018-05-01','2018-07-01', source);

    expect(result.length).toEqual(1);

});

it('FindApplicableStateChanges Includes Multiple Matches', () => {

    var source = [
      { start: '2018-06-03' , end: '2018-08-01'},
      { start: '2018-06-01' , end: '2018-06-02'}
  ];

    var result = DataUtilities.findApplicableStateChanges('2018-05-01','2018-07-01', source);

    expect(result.length).toEqual(2);

});


it('FindApplicableStateChanges Includes Future Matches', () => {

    var source = [
      { start: '2050-06-03' , end: '2050-08-01'}
  ];

    var result = DataUtilities.findApplicableStateChanges('2050-05-01','2050-07-01', source);

    expect(result.length).toEqual(1);

});


it('FindApplicableStateChanges Excludes Non Matches', () => {

    var source = [
      { start: '2018-06-03' , end: '2018-08-01'}
  ];

    var result = DataUtilities.findApplicableStateChanges('2018-03-01','2018-04-01', source);

    expect(result.length).toEqual(0);

});

/********************** FindApplicableEvents ******************/


it('FindApplicableEvents Includes Matches', () => {

    var source = [ { on: '2018-06-03' } ];

    var result = DataUtilities.findApplicableEvents('2018-05-01','2018-07-01', source);

    expect(result.length).toEqual(1);

});


it('FindApplicableEvents Includes Multiple Matches', () => {

    var source = [ { on: '2018-06-03' }, { on: '2018-06-04' } ];

    var result = DataUtilities.findApplicableEvents('2018-05-01','2018-07-01', source);

    expect(result.length).toEqual(2);

});


it('FindApplicableEvents Excludes Multiple Matches', () => {

    var source = [ { on: '2017-06-03' } ];

    var result = DataUtilities.findApplicableEvents('2018-05-01','2018-07-01', source);

    expect(result.length).toEqual(0);

});


it('FindApplicableEvents Is Inclusive Begin', () => {

    var source = [ { on: '2017-06-02' } ];

    var start = moment('2017-06-02');
    var end = start.clone().add(1,'days');

    var result = DataUtilities.findApplicableEvents(start,end, source);

    expect(result.length).toEqual(1);

});

it('FindApplicableEvents Is Inclusive End', () => {

    var source = [ { on: '2017-06-03' } ];

    var start = moment('2017-06-02');
    var end = start.clone().add(1,'days');

    var result = DataUtilities.findApplicableEvents(start,end, source);

    expect(result.length).toEqual(1);

});



/********************** GroupBy ******************/


it('GroupBy Basic Event Split', () => {

    var events = [ { on: '2017-06-03' } ];
    var stateChanges = [];

    var result = DataUtilities.groupBy(events, stateChanges, '2017-01-01', '2018-01-01', 'MMMM Do YYYY','days',1, false);

    expect(result.length).toEqual(1);
    expect(result[0].events.length).toEqual(1);

});


it('GroupBy Basic State Split', () => {

    var events = [ { on: '2017-06-03' } ];
    var stateChanges = { 'test' : [{ start: '2017-06-03 10:00' , end: '2017-06-03 11:00'}] };

    var result = DataUtilities.groupBy(events, stateChanges, '2017-01-01', '2018-01-01', 'MMMM Do YYYY','days',1, false);

    expect(result.length).toEqual(1);
    expect(result[0].events.length).toEqual(1);
    expect(result[0].stateChanges['test'].length).toEqual(1);
});
