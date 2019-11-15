//啊哦,this file no defind of old mfui,who use who write

export default {
    namespace: 'cloudServer',
    state: {
        
    },
    reducers: {
        setState(state, { payload }) {
            return {
                ...state,
                ...payload
            };
        }
    },
    effects: {
        
    },
    subscriptions: {
        
    }
};
