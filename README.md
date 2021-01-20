# Personal Metadata Exchanger

                 The C.A.B.A.L.
(Coalition Against Biased Argumentation Labyrinths)
                   presents

          Personal Metadata Exchanger.

@2021 Davide "limaCAT" Inglima

## What this should (hopefully) become (in 4 or 5 years)?

A text-only usenet client-server combo with extra perks.

## Which ones?

### Fast creation of GPG Signatures.

Not a keyring, but rather an "I take responsibility for whatever goes out on usenet with this signature".
    
### Robomoderation: A system of optional, shareable inbound / outbound bayes filters.

Every single PMX user should be able to filter messages fast by applying one or more bayes filters, each one
trained on a certain aspect. Note that every message that will pass on the inbound list will pass while the 
message while outbound messages list by design. In that case the user will have an "are you sure" message
popping up.

Notice that I will not distribute an antispam list on github, but eventually it should be possible to collect
spam samples. Also it will be based on a simple chain.

### Peer2Peer based distribution of news batches.

Instead of being a client, PMX should also be a peer-2-peer way to distribute TEXT-ONLY usenet news.
Reason: there's enough free bandwidth between home users for sustaining text-only news batches.

## Why text-only?

I don't aim this to become a particularly efficient usenet client, it's just a hobby.

## Why .net and not...

Technologically speaking the first best alternative would have been Rust, the second best C++.
I currently have not enough time to study either of those. End of story.

# Roadmap

* 2021 Reasonably implement the NNTP RFCS and have a basic windows client and a standalone server
* 2022 Signatures
* 2024 P2P Distribution of news batches
* 2023 Robomoderation / Bayes Filters