import React, { Component } from 'react';
import { Col, Button, Row, FormControl, Form, Modal } from 'react-bootstrap';
import * as _ from 'lodash';

const findMatchingFamily = (familiesState, familyToFind) => {
    return familiesState.find(family => family.name === familyToFind.name);
}

export class Home extends Component {
    displayName = Home.name
    constructor(props) {
        super(props);
        this.state = { families: [], pairs: [], showResults: false, errorMessage: null };
        this.addFamily = this.addFamily.bind(this);
        this.addFamilyMember = this.addFamilyMember.bind(this);
        this.memberNameChange = this.memberNameChange.bind(this);
        this.submitFamilies = this.submitFamilies.bind(this);
        this.closeModal = this.closeModal.bind(this);
    }

    addFamily() {
        this.setState(prevState => ({
            families: [...prevState.families, { name: `Family ${prevState.families.length + 1}`, members: [] }]
        }));
    }

    addFamilyMember(selectedFamily) {
        this.setState(prevState => {
            const returnState = _.cloneDeep(prevState);
            const currentFamily = findMatchingFamily(returnState.families, selectedFamily);
            currentFamily.members.push({ name: '' });
            return returnState;
        });
    }

    memberNameChange(selectedFamily, selectedMember, event) {
        event.persist();
        this.setState(prevState => {
            const returnState = _.cloneDeep(prevState);
            const currentFamily = findMatchingFamily(returnState.families, selectedFamily);
            const currentMember = currentFamily.members.find(member => member.name === selectedMember.name);
            currentMember.name = event.target.value;
            return returnState;
        });
    }

    async submitFamilies() {
        let results;
        if (this.state.families.some(family => family.members.length < 2)) {
            return;
        }
        try {
            results = await fetch('api/secretsanta/pair', {
                method: 'post',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(this.state.families)
            });
        } catch (e) {
            this.setState({
                errorMessage: e
            });
            return;
        }

        const resultsJson = await results.json();
        if (results.status !== 200) {
            this.setState({
                errorMessage: resultsJson.message
            });

            return;
        }
        this.setState({
            showResults: true,
            pairs: resultsJson.pairs
        });

    }

    closeModal() {
        this.setState({ showResults: false });
    }

    render() {
        return (
            <div>
                <Row>
                    <Col>
                        <h2>Please enter families and members below:</h2>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <Button onClick={this.addFamily}>Click to add Family</Button>
                    </Col>
                </Row>
                <Row>
                    {
                        this.state.families.length > 0 &&
                        this.state.families.map(family =>
                            (
                                <Form bsSize="large">
                                    <h3>{family.name}</h3>
                                    {
                                        family.members.length > 0 &&
                                        family.members.map(member =>
                                            (
                                                <FormControl
                                                    style={{ marginTop: '15px' }}
                                                    type="text"
                                                    value={member.name}
                                                    onChange={(e) => this.memberNameChange(family, member, e)}
                                                />
                                            )
                                        )
                                    }
                                    <Button style={{ marginTop: '15px' }} onClick={() => this.addFamilyMember(family)}>Click to add member</Button>
                                    {
                                        family.members.length < 2 &&
                                        <p>Please enter 2 members for each family!</p>
                                    }
                                </Form>
                            )
                        )
                    }
                </Row>
                <Row style={{ marginTop: '15px' }}>
                    <Button
                        bsStyle="primary"
                        onClick={this.submitFamilies}>Click to generate pairs!</Button>
                    {
                        this.state.errorMessage &&
                        <p style={{
                            marginTop: '15px'
                        }}>{this.state.errorMessage}</p>
                    }
                </Row>
                <Modal show={this.state.showResults} onHide={this.closeModal}>
                    <Modal.Header closeButton>
                        <Modal.Title>
                            Generate Successful! See results below!
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        {
                            this.state.pairs.map(pair => (
                                <p>{pair.giver.name} -> {pair.recipient.name}</p>
                            ))
                        }
                    </Modal.Body>
                </Modal>
            </div>
        );
    }
}
