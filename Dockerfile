FROM mcr.microsoft.com/mssql/server:2019-latest
ENV SA_PASSWORD testPassword@
ENV ACCEPT_EULA Y
EXPOSE 1433

USER root

RUN sed -i 's/archive.ubuntu.com/mirror.kakao.com/g' /etc/apt/sources.list
WORKDIR /setup
RUN apt update
RUN apt install -y wget
RUN wget "https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb"
RUN dpkg -i packages-microsoft-prod.deb
RUN apt update
RUN apt install -y apt-transport-https dotnet-sdk-5.0
# Cleanup
RUN rm -rf *.deb

WORKDIR /source
COPY ["./", "./"]
RUN wget https://gist.githubusercontent.com/KangDroid/6d2c8eef8b4438e10627f41edcf0132a/raw/46885de316c648d464b9267e1bccb59638f4abac/run_dotnet_test.sh
RUN chmod a+x run_dotnet_test.sh
ENTRYPOINT ["/bin/bash", "/source/run_dotnet_test.sh"]